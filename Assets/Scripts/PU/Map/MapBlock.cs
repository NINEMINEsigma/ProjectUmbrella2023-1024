using System;
using System.Collections;
using System.Collections.Generic;
using PU.Card.Buff;
using UnityEngine;

namespace PU.Map
{
    public class Block
    {
        public string _m_name = "default";
        public string _m_description = "default";

        public int[] _m_location = new int[2];
        public int LocationX => _m_location[0];
        public int LocationY => _m_location[1];

        public Block[] _m_leafs = new Block[4] { null, null, null, null };

        public List<PU.Card.Buff.Buff> _m_buffs = new();

        public PU.Card.Character character = null;

        public Block()
        {
        }

        public Block(string name, string description, int[] location, Block[] leafs, List<Buff> buffs)
        {
            _m_name = name;
            _m_description = description;
            _m_location = location;
            _m_leafs = leafs;
            _m_buffs = buffs;
        }

        public Block(string name, string description = "")
        {
            _m_name = name;
            _m_description = description;
        }
    }

    public class RootBlock
    {
        public string _m_name = "default";
        public string _m_description = "default";

        public Block _m_root;
        public Dictionary<int[], Block> _m_blocks = new();

        public RootBlock(int max_x, int max_y)
        {
            int _x = max_x;
            Block current = null;
            while (_x-- > 0)
            {
                if (current == null)
                {
                    _m_root = current = new();
                }
                else
                {
                    current._m_leafs[1] = new();
                    current._m_leafs[1]._m_leafs[3] = current;
                    current = current._m_leafs[1];
                }
                Block cat = current;
                for (int _y = max_y - 1; _y > 0; _y--)
                {
                    cat._m_leafs[0] = new();
                    cat._m_leafs[0]._m_leafs[2] = cat;
                    cat = cat._m_leafs[0];
                }

            }
        }
    }
}