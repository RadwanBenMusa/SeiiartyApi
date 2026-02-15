namespace TamaApi.clsMod
{
    public class ClsModWhatsAppMessage
    {
        public ClsModWhatsAppMessage(string messaging_product, string to, string type, Template template)
        {
            this.messaging_product = messaging_product;
            this.to = to;
            this.type = type;
            this.template = template;
        }

        public string messaging_product { get; set; }
        public string to { get; set; }
        public string type { get; set; }
        public Template template { get; set; }
    }
    public class Template
    {
        public Template(string name, Language language, List<Component> components)
        {
            this.name = name;
            this.language = language;
            this.components = components;
        }

        public string name { get; set; }
        public Language language { get; set; }
        public List<Component> components { get; set; }
    }
    public class Language
    {
        public string? code { get; set; }
    }
    public class Component
    {
        public string? type { get; set; }
        public List<Parameter>? parameters { get; set; }
    }
    public class Parameter
    {
        public string? type { get; set; }
        public Document? document { get; set; }
        public string? text { get; set; }
    }
    public class Document
    {
        public string? link { get; set; }
        public string? filename { get; set; }

    }
}
