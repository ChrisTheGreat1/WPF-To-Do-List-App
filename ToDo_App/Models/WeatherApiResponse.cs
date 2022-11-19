namespace ToDo_App.Models
{
    public class Main
    {
        public float feels_like { get; set; }
        public float temp { get; set; }
        public float temp_max { get; set; }
        public float temp_min { get; set; }
    }

    public class Sys
    {
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class Weather
    {
        public string? description { get; set; }
        public string? icon { get; set; }
        public string? main { get; set; }
    }

    public class WeatherApiResponse
    {
        public Main? main { get; set; }
        public string? name { get; set; }
        public Sys? sys { get; set; }
        public Weather[]? weather { get; set; }
        public Wind? wind { get; set; }
    }

    public class Wind
    {
        public float speed { get; set; }
    }
}