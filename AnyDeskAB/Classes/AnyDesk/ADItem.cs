using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AnyDeskAB.Classes.AnyDesk {
    public abstract class ADItem : IADItem {
        public event EventHandler NameChanged;

        private ADItem mParent;
        private string mID;
        private string mName;

        public ADItem(ADItem parent, string id, string name) {
            mParent = parent;
            mID = id;
            mName = name;
        }

        public string Id { get { return mID; } }

        public string Name {
            get { return mName; }
            set {
                mName = value;
                NameChanged?.Invoke(this, new EventArgs());
            }
        }

        public ADItem Parent {
            get { return mParent; }
        }

        public virtual XElement ToXML() {
            //<?xml version=""1.0"" encoding=""utf-8""?>
            string xml = $@"<adItem>
                                <id>{mID}</id>
                                <name>{(new XText(mName)).ToString()}</name>
                            </adItem>";
            return XElement.Parse(xml);
        }

        public static bool operator ==(ADItem adi1, ADItem adi2) {
            if(adi1 is null && adi2 is null) return true;
            if(adi1 is null && !(adi2 is null)) return false;
            if(!(adi1 is null) && adi2 is null) return false;
            return adi1.Id == adi2.Id;
        }

        public static bool operator !=(ADItem adi1, ADItem adi2) {
            return !(adi1 == adi2);
        }

        public override bool Equals(object obj) {
            if(!(obj is ADItem)) return false;
            return this == (ADItem)obj;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public abstract ADItem Clone(ADItem parent);
    }
}
