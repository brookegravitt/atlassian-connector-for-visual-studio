namespace Atlassian.plvs.api.bamboo {
    public class BambooPlan {
        public BambooPlan(string key, string name, bool enabled) {
            Key = key;
            Name = name;
            Enabled = enabled;
        }

        public string Key { get; private set; }
        public string Name { get; private set; }
        public bool? Favourite { get; set; }
        public bool Enabled { get; private set; }
    }
}
