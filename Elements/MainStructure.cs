namespace PageBuilder.Elements
{
    public class MainStructure : TemplateElement
    {
        string Ani;
        string Sty;
        string ContenCode;
        public MainStructure(string ani, string sty, string contentCode)
        {
            Ani = ani;
            Sty = sty;
            ContenCode = contentCode;
        }
        public override DataPairs GetData() => new DataPairs()
        {
            { "styles",Sty},
            { "animations",Ani},
            { "content",ContenCode }
        };
    }
}
