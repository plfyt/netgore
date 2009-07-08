using System;
using Microsoft.Xna.Framework;
using NetGore;

namespace DemoGame.Server
{
    public class ItemTemplate
    {
        readonly string _desc;
        readonly GrhIndex _graphic;
        readonly byte _height;
        readonly ItemTemplateID _id;
        readonly string _name;
        readonly ItemStats _stats;
        readonly ItemType _type;
        readonly int _value;
        readonly byte _width;

        public string Description
        {
            get { return _desc; }
        }

        public GrhIndex Graphic
        {
            get { return _graphic; }
        }

        public byte Height
        {
            get { return _height; }
        }

        public ItemTemplateID ID
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public Vector2 Size
        {
            get { return new Vector2(_width, _height); }
        }

        public ItemStats Stats
        {
            get { return _stats; }
        }

        public ItemType Type
        {
            get { return _type; }
        }

        public int Value
        {
            get { return _value; }
        }

        public byte Width
        {
            get { return _width; }
        }

        public ItemTemplate(ItemTemplateID id, string name, string desc, ItemType type, GrhIndex graphic, int value, byte width,
                            byte height, ItemStats stats)
        {
            _id = id;
            _name = name;
            _desc = desc;
            _type = type;
            _graphic = graphic;
            _value = value;
            _width = width;
            _height = height;
            _stats = stats;

            // Make sure the ItemType is defined
            if (!type.IsDefined())
            {
                const string errmsg = "Invalid ItemType `{0}` for ItemTemplate ID `{1}`.";
                throw new InvalidCastException(string.Format(errmsg, type, id));
            }
        }

        public ItemEntity CreateInstance(byte amount)
        {
            ItemEntity instance = new ItemEntity(this, Vector2.Zero, amount);
            return instance;
        }

        public ItemEntity CreateInstance(Map map, Vector2 position, byte amount)
        {
            ItemEntity instance = new ItemEntity(this, position, amount);
            map.AddEntity(instance);
            return instance;
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Name, ID);
        }
    }
}