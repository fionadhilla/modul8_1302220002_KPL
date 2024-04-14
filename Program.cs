using System.Text.Json;

public class CovidConfig
{
    public string satuan_suhu { get; set; }
    public int batas_hari_demam { get; set; }
    public string pesan_ditolak { get; set; }
    public string pesan_diterima { get; set; }
}

public class Program
{
    public static void Main(string[] args)
    {
        CovidConfig config = ReadJSON();

        Console.WriteLine($"Berapa suhu badan anda saat ini? dalam nilai {config.satuan_suhu}");
        string input = Console.ReadLine();

        double suhu = UbahTemperature(input, out bool isCelsius);

        Console.WriteLine($"Berapa hari yang lalu (perkiraan) anda terakhir memiliki gejala demam?");
        int demam = Convert.ToInt32(Console.ReadLine());

        bool isApproved = (isCelsius ? (suhu >= 36.5 && suhu <= 37.5) : (suhu >= 97.7 && suhu <= 99.5)) && demam < config.batas_hari_demam;
        string message = isApproved ? config.pesan_diterima : config.pesan_ditolak;

        Console.WriteLine(message);
    }

    private static double UbahTemperature(string input, out bool isCelsius)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Input tidak boleh null atau kosong");
        }

        string[] parts = input.Split(' ');
        if (parts.Length != 2)
        {
            throw new ArgumentException("Input harus dalam format <'nilai suhu' 'satuan suhu'>");
        }

        double value;
        if (!double.TryParse(parts[0], out value))
        {
            throw new ArgumentException("Nilai harus berupa angka");
        }

        string unit = parts[1].ToLower();
        if (unit != "celcius" && unit != "fahrenheit")
        {
            throw new ArgumentException("Satuan harus 'celcius' atau 'fahrenheit'");
        }

        isCelsius = unit == "celcius";

        if (isCelsius && (value < 36.5 || value > 37.5))
        {
            throw new ArgumentException("Suhu Celcius harus antara 36.5 dan 37.5");
        }

        if (!isCelsius && (value < 97.7 || value > 99.5))
        {
            throw new ArgumentException("Suhu Fahrenheit harus antara 97.7 dan 99.5");
        }

        return value;
    }

    private static CovidConfig ReadJSON()
    {
        string configPath = "../../../covid_config.json";
        //if (!File.Exists(configPath))
        //{
         //   return new CovidConfig
        //    {
         //       satuan_suhu = "celcius",
       //         batas_hari_demam = 14,
       //         pesan_ditolak = "Anda tidak diperbolehkan masuk ke dalam gedung ini",
        //        pesan_diterima = "Anda dipersilahkan untuk masuk ke dalam gedung ini"
        //    };
     //   }

        string jsonString = File.ReadAllText(configPath);
        return JsonSerializer.Deserialize<CovidConfig>(jsonString);
    }
}
