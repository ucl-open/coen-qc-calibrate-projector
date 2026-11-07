using Bonsai;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using MathNet.Numerics;

[Combinator]
[Description("Fits a projector gamma lookup table to a brightness ramp and writes it beside rig.yml: calibration\\<FileName>.bmp, the one-row bitmap BonVision's GammaCorrection reads, and calibration\\<FileName>.json, the ramp and who ran it, each with a dated copy in calibration\\history\\. The lookup it replaces is kept in history\\ first.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class WriteGammaLookup
{
    public WriteGammaLookup()
    {
        FileName = "gammalut";
        CalibratedBy = "unknown";
    }

    [Description("The machine's configuration directory, <ConfigRoot>\\<MACHINE-NAME>, where rig.yml is.")]
    public string Directory { get; set; }

    [Description("The file name without extension, written into calibration\\.")]
    public string FileName { get; set; }

    [Description("Who ran the calibration, kept in the JSON record.")]
    public string CalibratedBy { get; set; }

    public IObservable<string> Process(IObservable<Tuple<double, double>[]> source)
    {
        return source.Select(Write);
    }

    string Write(Tuple<double, double>[] ramp)
    {
        if (string.IsNullOrEmpty(Directory)) throw new InvalidOperationException("No configuration directory to write the gamma lookup into.");
        if (ramp.Length < 3) throw new InvalidOperationException(string.Format("A gamma lookup needs at least 3 levels; the ramp had {0}.", ramp.Length));
        var levels = ramp.Select(p => p.Item1).ToArray();
        var readings = ramp.Select(p => p.Item2).ToArray();

        // Brightness above the dark screen, as a fraction of full white. A ramp that did not get
        // brighter would still fit, so the display would go black or grey with no reason given.
        var dark = readings[0];
        var span = readings.Max() - dark;
        if (readings[readings.Length - 1] - dark <= 0 || span <= 0)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                "The ramp got no brighter: the photodiode read {0:F1} at level 0 and {1:F1} at full white. Check it faces the screen and is on the analog input the workflow reads.",
                dark, readings[readings.Length - 1]));
        }
        var brightness = readings.Select(r => (r - dark) / span).ToArray();

        // The table: for each wanted brightness, the level that gave it, interpolated between the two
        // measured levels around it. A polynomial fit misses the steep start of the curve by up to
        // 6/255. Noise that dips the measured curve is flattened first, so the table never falls.
        var rising = new double[brightness.Length];
        for (int i = 0; i < rising.Length; i++) rising[i] = Math.Max(brightness[i], i > 0 ? rising[i - 1] : 0);
        var lookup = levels.Select(target => LevelFor(target, rising, levels)).ToArray();
        lookup[0] = 0;
        lookup[lookup.Length - 1] = 1;

        var calibration = Path.Combine(Directory, "calibration");
        var history = Path.Combine(calibration, "history");
        System.IO.Directory.CreateDirectory(history);
        var bmp = Path.Combine(calibration, FileName + ".bmp");
        var json = Path.Combine(calibration, FileName + ".json");
        Archive(bmp, history);
        Archive(json, history);

        var now = DateTime.UtcNow;
        File.WriteAllBytes(bmp, Bitmap(lookup));
        File.WriteAllText(json, Record(now, levels, readings, lookup), new UTF8Encoding(false));
        var stamp = now.ToString("yyyy-MM-ddTHHmmss'Z'", CultureInfo.InvariantCulture);
        File.Copy(bmp, Path.Combine(history, FileName + "_" + stamp + ".bmp"), true);
        File.Copy(json, Path.Combine(history, FileName + "_" + stamp + ".json"), true);

        return string.Format(CultureInfo.InvariantCulture, "{0} levels, gamma about {1:F2}, dark {2:F1}, white {3:F1}: calibration/{4}.bmp",
            levels.Length, Gamma(levels, brightness), dark, readings.Max(), FileName);
    }

    static double LevelFor(double target, double[] rising, double[] levels)
    {
        for (int k = 0; k < rising.Length - 1; k++)
        {
            if (target <= rising[k + 1])
            {
                var step = rising[k + 1] - rising[k];
                return step <= 0 ? levels[k] : levels[k] + (target - rising[k]) / step * (levels[k + 1] - levels[k]);
            }
        }
        return 1;
    }

    // The file being replaced, into history\ under its own date, unless a copy is already there.
    static void Archive(string path, string history)
    {
        if (!File.Exists(path)) return;
        var stamp = File.GetLastWriteTimeUtc(path).ToString("yyyy-MM-ddTHHmmss'Z'", CultureInfo.InvariantCulture);
        var copy = Path.Combine(history, Path.GetFileNameWithoutExtension(path) + "_" + stamp + Path.GetExtension(path));
        if (!File.Exists(copy)) File.Copy(path, copy);
    }

    // The exponent of brightness = level^gamma, by least squares on the log-log ramp.
    static double Gamma(double[] levels, double[] brightness)
    {
        var points = levels.Zip(brightness, (l, b) => Tuple.Create(l, b)).Where(p => p.Item1 > 0.05 && p.Item2 > 0.001).ToArray();
        if (points.Length < 2) return double.NaN;
        var fit = Fit.Line(points.Select(p => Math.Log(p.Item1)).ToArray(), points.Select(p => Math.Log(p.Item2)).ToArray());
        return fit.Item2;
    }

    // A one-row, 24-bit uncompressed BMP, each pixel grey at its level's drive.
    static byte[] Bitmap(double[] lookup)
    {
        var row = (lookup.Length * 3 + 3) / 4 * 4;
        var data = new byte[54 + row];
        var header = new MemoryStream(data);
        var w = new BinaryWriter(header);
        w.Write((byte)'B'); w.Write((byte)'M'); w.Write(data.Length); w.Write(0); w.Write(54);
        w.Write(40); w.Write(lookup.Length); w.Write(1); w.Write((short)1); w.Write((short)24);
        w.Write(0); w.Write(row); w.Write(2835); w.Write(2835); w.Write(0); w.Write(0);
        for (int i = 0; i < lookup.Length; i++)
        {
            var grey = (byte)Math.Round(255 * lookup[i]);
            data[54 + 3 * i] = data[55 + 3 * i] = data[56 + 3 * i] = grey;
        }
        return data;
    }

    string Record(DateTime now, double[] levels, double[] readings, double[] lookup)
    {
        Func<double[], string> list = values => "[" + string.Join(", ", values.Select(v => v.ToString("R", CultureInfo.InvariantCulture))) + "]";
        Func<string, string> text = s => "\"" + (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
        return "{\n" +
            "  \"calibratedAt\": " + text(now.ToString("yyyy-MM-ddTHH:mm:ss'Z'", CultureInfo.InvariantCulture)) + ",\n" +
            "  \"calibratedBy\": " + text(CalibratedBy) + ",\n" +
            "  \"machineName\": " + text(Environment.MachineName) + ",\n" +
            "  \"lookup\": " + text(FileName + ".bmp") + ",\n" +
            "  \"levels\": " + list(levels) + ",\n" +
            "  \"readings\": " + list(readings) + ",\n" +
            "  \"drive\": " + list(lookup) + "\n" +
            "}\n";
    }
}
