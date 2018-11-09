using AnyDeskAB.Classes.AnyDesk;
using System.Linq;
using System.Xml.Linq;

namespace AnyDeskAB.Classes {
    public class Item : ADItem {
        private string mAddress;
        private string mAlias;
        private string mDescription;

        public Item(ADItem parent, string id, string address, string alias) : base(parent, id, alias) {
            mAddress = address;
            mAlias = alias;
            mDescription = "";

            base.NameChanged += delegate { mAlias = base.Name; };
        }

        public string Address {
            get { return mAddress; }
        }

        public string Alias {
            get { return mAlias; }
            set { mAlias = value; }
        }

        public string Description {
            get { return mDescription; }
            set { mDescription = value; }
        }

        public override string ToString() {
            return mAlias == "" ? mAddress : mAlias;
        }

        public override XElement ToXML() {
            XElement xmlBase = base.ToXML();

            string xml = $@"<item>
                                <address>{mAddress}</address>
                                <alias>{mAlias}</alias>
                                <description>{mDescription}</description>
                            </item>";

            xmlBase.Add(XElement.Parse(xml));
            return xmlBase;
        }

        public static Item FromXML(ADItem parent, XElement xml) {
            string id = xml.Elements("id").First().Value;
            XElement xmlItem = xml.Elements("item").First();
            string address = xmlItem.Elements("address").First().Value;
            string alias = xmlItem.Elements("alias").First().Value;
            Item i = new Item(parent, id, address, alias) {
                Description = xmlItem.Elements("description").First().Value
            };
            return i;
        }

        public override ADItem Clone(ADItem parent) {
            return Item.FromXML(parent, this.ToXML());
        }
    }
}
