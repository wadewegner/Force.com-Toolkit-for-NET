namespace ForceToolkitForNET.Models
{
    public class SObject
    {
        public string name { get; set; }
        public string labelPlural { get; set; }
        public bool layoutable { get; set; }
        public bool activateable { get; set; }
        public bool searchable { get; set; }
        public bool custom { get; set; }
        public Urls urls { get; set; }
        public bool queryable { get; set; }
        public bool createable { get; set; }
        public bool deletable { get; set; }
        public bool updateable { get; set; }
        public string keyPrefix { get; set; }
        public bool customSetting { get; set; }
        public bool undeletable { get; set; }
        public bool mergeable { get; set; }
        public bool replicateable { get; set; }
        public bool triggerable { get; set; }
        public bool feedEnabled { get; set; }
        public bool retrieveable { get; set; }
        public bool deprecatedAndHidden { get; set; }
        public string label { get; set; }
    }
}