using AnyDeskAB.Classes.AnyDesk;
using System.Linq;
using System.Xml.Linq;

namespace AnyDeskAB.Classes {
    public class Item : ADItem {
        private string mAddress;
        private string mAlias;
        private string mDescription;
        private string mThumbnailId;

        public Item(ADItem parent, string id, string address, string alias, string thumbnailId) : base(parent, id, alias) {
            mAddress = address;
            mAlias = alias;
            mDescription = "";
            mThumbnailId = thumbnailId;

            base.NameChanged += delegate { mAlias = base.Name; };
        }

        public string Address { get => mAddress; }

        public string ThumbnailId {
            get => mThumbnailId;
            set => mThumbnailId = value;
        }

        public string Alias {
            get => mAlias;
            set => mAlias = value;
        }

        public string Description {
            get => mDescription;
            set => mDescription = value;
        }

        public override string ToString() {
            return string.IsNullOrEmpty(mAlias) ? mAddress : mAlias;
        }

        public override XElement ToXML() {
            XElement xmlBase = base.ToXML();

            string xml = $@"<item>
                                <address>{mAddress}</address>
                                <alias>{mAlias}</alias>
                                <description>{mDescription}</description>
                                <thumbnailId>{mThumbnailId}</thumbnailId>
                            </item>";

            xmlBase.Add(XElement.Parse(xml));
            return xmlBase;
        }

        public static Item FromXML(ADItem parent, XElement xml) {
            string thumbnailId = "";

            string id = xml.Elements("id").First().Value;
            XElement xmlItem = xml.Elements("item").First();
            string address = xmlItem.Elements("address").First().Value;
            string alias = xmlItem.Elements("alias").First().Value;
            try { thumbnailId = xmlItem.Elements("thumbnailId").First().Value; } catch { }
            Item i = new Item(parent, id, address, alias, thumbnailId) {
                Description = xmlItem.Elements("description").First().Value
            };
            return i;
        }

        public override ADItem Clone(ADItem parent) {
            return Item.FromXML(parent, this.ToXML());
        }
    }
}
