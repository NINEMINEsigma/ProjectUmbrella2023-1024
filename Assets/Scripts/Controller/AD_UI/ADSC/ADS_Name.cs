using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Item.UI.Appendage
{
    public class ADS_Name : Item
    {
        public enum Type
        {
            int_,
            float_,
            bool_,
            two
        }

        TMP_Text name_;
        TMP_Text Name
        {
            get
            {
                if (name_ == null) name_ = GetComponent<TMP_Text>();
                return name_;
            }
        }
        [SerializeField] Type type;
        public Vector2 range;
        float size => range.y - range.x;
        [SerializeField] string on_, off_, attribute;

        public override void Init(Item_Map from)
        {
            throw new Main.MainSystemException("ADS_Name cannt initmap");
        }

        public override bool IsIgnore() { return true; }

        private void Start()
        {
            OnChange();
        }

        public void OnChange()
        {
            Add(T =>
            {
                switch (type)
                {
                    case Type.bool_:
                        Name.text = (GetValue() == 1) ? on_ : off_;
                        break;
                    case Type.float_:
                        Name.text = attribute + ":" + GetValue().ToString();
                        break;
                    case Type.int_:
                        Name.text = attribute + ":" + GetValue().ToString();
                        break;
                    case Type.two:
                        Name.text = attribute + ":" + GetValue().ToString() + "/" + size.ToString();
                        break;
                }
                T.state = Base.State.Destroy;
            });
        }

        public override float GetValue()
        {
            switch (type)
            {
                case Type.int_:
                    return (int)(size * ItemValue + range.x);
                case Type.float_:
                    return (size * ItemValue + range.x);
                case Type.bool_:
                    return (ItemValue > 0.5) ? 1 : 0;
                case Type.two:
                    return (size * ItemValue + range.x);
                default:return 0;
            }
        }

        public override Item_Map ToMap()
        {
            throw new Main.MainSystemException("ADS_Name cannt tomap");
        }
    }
}