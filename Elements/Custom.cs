namespace PageBuilder.Elements
{
    public class Custom : IPGElement
    {
        public string Xaml { get; set; }
        public string GetXaml() => Xaml;
        public Custom(string xaml)
        {
            Xaml = xaml;
        }
    }
}
