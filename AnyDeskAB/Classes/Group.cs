using AnyDeskAB.Classes.AnyDesk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AnyDeskAB.Classes {
    public class Group : ADItem {
        private readonly List<Item> mItems;
        private readonly List<Group> mGroups;

        public Group(ADItem parent, string name) : base(parent, Guid.NewGuid().ToString(), name) {
            mItems = new List<Item>();
            mGroups = new List<Group>();
        }

        public List<Item> Items { get => mItems; }
        public List<Group> Groups { get => mGroups; }

        public override XElement ToXML() {
            XElement xmlBase = base.ToXML();

            string xml = $@"<group>
                                <items>{string.Concat(from Item i in mItems select i.ToXML().ToString())}</items>
                                <groups>{string.Concat(from Group g in mGroups select g.ToXML().ToString())}</groups>
                            </group>";

            xmlBase.Add(XElement.Parse(xml));
            return xmlBase;
        }

        public static Group FromXML(ADItem parent, XElement xml) {
            Group g = new Group(parent, xml.Elements("name").First().Value);
            foreach(XElement xmlAdItem in xml.Elements("group").Elements("items").Elements("adItem")) {
                g.Items.Add(Item.FromXML(g, xmlAdItem));
            }
            foreach(XElement xmlGrpItem in xml.Elements("group").Elements("groups").Elements("adItem")) {
                g.Groups.Add(Group.FromXML(g, xmlGrpItem));
            }
            return g;
        }

        public override ADItem Clone(ADItem parent) {
            return Group.FromXML(parent, this.ToXML());
        }

        public override string ToString() {
            return base.Name;
        }

        public Item FindItem(Item item) {
            foreach(Item i in mItems) {
                if(i == item) return i;
            }

            foreach(Group g in mGroups) {
                foreach(Item i in mItems) {
                    Item ti = g.FindItem(item);
                    if(ti != null) return ti;
                }
            }

            return null;
        }

        public bool ItemExists(Item item) {
            return FindItem(item) != null;
        }

        public List<Item> GetAllItems() {
            return GetAllItems(this);
        }

        private List<Item> GetAllItems(Group g) {
            List<Item> items = new List<Item>();
            items.AddRange(mItems.ToArray());

            foreach(Group sg in mGroups) {
                items.AddRange(sg.GetAllItems());
            }

            return items;
        }
    }
}