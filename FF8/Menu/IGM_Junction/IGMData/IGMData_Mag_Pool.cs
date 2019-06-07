﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FF8
{
    public partial class Module_main_menu_debug
    {
        private partial class IGM_Junction
        {
            private class IGMData_Mag_Pool : IGMData_Pool<Saves.CharacterData, byte>
            {
                public IGMData_Mag_Stat_Slots Stat_Slots { get; private set; }
                public IGMData_Mag_EL_A_D_Slots EL_A_D_Slots { get; private set; }
                public IGMData_Mag_ST_A_D_Slots ST_A_D_Slots { get; private set; }
                public IGMData_Mag_EL_A_Values EL_A_Values { get; private set; }
                public IGMData_Mag_EL_D_Values EL_D_Values { get; private set; }
                public IGMData_Mag_ST_A_Values ST_A_Values { get; private set; }
                public IGMData_Mag_ST_D_Values ST_D_Values { get; private set; }
                public Mode SortMode { get; private set; }
                public Mode LastMode { get; private set; }
                public Characters LastCharacter { get; private set; }
                public Kernel_bin.Stat LastStat { get; private set; }
                public int LastPage { get; private set; }
                public Kernel_bin.Stat Stat { get; private set; }
                public IEnumerable<Kernel_bin.Magic_Data> Sort { get; private set; }

                public IGMData_Mag_Pool() : base(5, 3, new IGMDataItem_Box(pos: new Rectangle(135, 150, 300, 192), title: Icons.ID.MAGIC), 4, 13)
                {
                }

                protected override void InitShift(int i, int col, int row)
                {
                    base.InitShift(i, col, row);
                    SIZE[i].Inflate(-22, -8);
                    SIZE[i].Offset(0, 12 + (-8 * row));
                }

                private void addMagic(ref int pos, Kernel_bin.Magic_Data spell, Font.ColorID color = Font.ColorID.White)
                {
                    ITEM[pos, 0] = new IGMDataItem_String(spell.Name, SIZE[pos], color);
                    ITEM[pos, 1] = color != Font.ColorID.White ? new IGMDataItem_Icon(Icons.ID.JunctionSYM, new Rectangle(SIZE[pos].X + SIZE[pos].Width - 75, SIZE[pos].Y, 0, 0)) : null;
                    ITEM[pos, 2] = new IGMDataItem_Int(Source.Magics[spell.ID], new Rectangle(SIZE[pos].X + SIZE[pos].Width - 50, SIZE[pos].Y, 0, 0), spaces: 3);
                    BLANKS[pos] = color == Font.ColorID.Dark_Gray ? true : false;
                    Contents[pos] = spell.ID;
                    pos++;
                }

                protected override void Init()
                {
                    base.Init();
                    SIZE[rows] = SIZE[0];
                    SIZE[rows].Y = Y;
                    ITEM[rows, 2] = new IGMDataItem_Icon(Icons.ID.NUM_, new Rectangle(SIZE[rows].X + SIZE[rows].Width - 45, SIZE[rows].Y, 0, 0), scale: new Vector2(2.5f));
                    BLANKS[rows] = true;
                    Cursor_Status &= ~Cursor_Status.Horizontal;
                    Cursor_Status |= Cursor_Status.Vertical;
                    Cursor_Status &= ~Cursor_Status.Blinking;
                }

                private void Get_Sort_Stat()
                {
                    Stat = Kernel_bin.Stat.None;
                    SortMode = InGameMenu_Junction.mode;
                    switch (SortMode)
                    {
                        default:
                        case Mode.Mag_Stat:
                        case Mode.Mag_Pool_Stat:
                            if (Stat_Slots != null)
                                Stat = Stat_Slots.Contents[Stat_Slots.CURSOR_SELECT];
                            SortMode = Mode.Mag_Pool_Stat;
                            break;

                        case Mode.Mag_ST_D:
                        case Mode.Mag_Pool_ST_D:
                            SortMode = Mode.Mag_Pool_ST_D;
                            Stat = Kernel_bin.Stat.EL_Def_1;
                            break;

                        case Mode.Mag_ST_A:
                        case Mode.Mag_Pool_ST_A:
                            SortMode = Mode.Mag_Pool_ST_A;
                            Stat = Kernel_bin.Stat.ST_Atk;
                            break;

                        case Mode.Mag_EL_D:
                        case Mode.Mag_Pool_EL_D:
                            SortMode = Mode.Mag_Pool_EL_D;
                            Stat = Kernel_bin.Stat.EL_Def_1;
                            break;

                        case Mode.Mag_EL_A:
                        case Mode.Mag_Pool_EL_A:
                            SortMode = Mode.Mag_Pool_EL_A;
                            Stat = Kernel_bin.Stat.EL_Atk;
                            break;
                    }
                }

                public void Get_Slots_Values()
                {
                    Stat_Slots = (IGMData_Mag_Stat_Slots)((IGMDataItem_IGMData)((IGMData_Mag_Group)InGameMenu_Junction.Data[SectionName.Mag_Group]).ITEM[0, 0]).Data;
                    EL_A_D_Slots = (IGMData_Mag_EL_A_D_Slots)((IGMDataItem_IGMData)((IGMData_Mag_Group)InGameMenu_Junction.Data[SectionName.Mag_Group]).ITEM[3, 0]).Data;
                    ST_A_D_Slots = (IGMData_Mag_ST_A_D_Slots)((IGMDataItem_IGMData)((IGMData_Mag_Group)InGameMenu_Junction.Data[SectionName.Mag_Group]).ITEM[6, 0]).Data;
                    EL_A_Values = (IGMData_Mag_EL_A_Values)((IGMDataItem_IGMData)((IGMData_Mag_Group)InGameMenu_Junction.Data[SectionName.Mag_Group]).ITEM[4, 0]).Data;
                    EL_D_Values = (IGMData_Mag_EL_D_Values)((IGMDataItem_IGMData)((IGMData_Mag_Group)InGameMenu_Junction.Data[SectionName.Mag_Group]).ITEM[5, 0]).Data;
                    ST_A_Values = (IGMData_Mag_ST_A_Values)((IGMDataItem_IGMData)((IGMData_Mag_Group)InGameMenu_Junction.Data[SectionName.Mag_Group]).ITEM[7, 0]).Data;
                    ST_D_Values = (IGMData_Mag_ST_D_Values)((IGMDataItem_IGMData)((IGMData_Mag_Group)InGameMenu_Junction.Data[SectionName.Mag_Group]).ITEM[8, 0]).Data;
                    Source = Memory.State.Characters[Character];
                }

                public void Get_Sort()
                {
                    switch (SortMode)
                    {
                        case Mode.Mag_Pool_Stat:
                            if (Stat != Kernel_bin.Stat.None)
                                Sort = Kernel_bin.MagicData.OrderBy(x => (-x.J_Val[Stat] * (Source.Magics.ContainsKey(x.ID) ? Source.Magics[x.ID] : 0)) / 100);
                            //Kernel_bin.MagicData.ToList().ConvertAll(x => (x.J_Val[Stat] * (Source.Magics.ContainsKey(x.ID) ? Source.Magics[x.ID] : 0)) / 100);
                            break;

                        case Mode.Mag_Pool_EL_D:
                            Sort = Kernel_bin.MagicData.OrderBy(x => (-x.J_Val[Kernel_bin.Stat.EL_Def_1] * x.Elem_J_def.Count() * (Source.Magics.ContainsKey(x.ID) ? Source.Magics[x.ID] : 0)) / 100);
                            break;

                        case Mode.Mag_Pool_EL_A:
                            Sort = Kernel_bin.MagicData.OrderBy(x => (-x.J_Val[Kernel_bin.Stat.EL_Atk] * x.Elem_J_atk.Count() * (Source.Magics.ContainsKey(x.ID) ? Source.Magics[x.ID] : 0)) / 100);
                            break;

                        case Mode.Mag_Pool_ST_D:
                            Sort = Kernel_bin.MagicData.OrderBy(x => (-x.J_Val[Kernel_bin.Stat.ST_Def_1] * x.Stat_J_def.Count() * (Source.Magics.ContainsKey(x.ID) ? Source.Magics[x.ID] : 0)) / 100);
                            break;

                        case Mode.Mag_Pool_ST_A:
                            Sort = Kernel_bin.MagicData.OrderBy(x => (-x.J_Val[Kernel_bin.Stat.ST_Atk] * x.Stat_J_atk.Count() * (Source.Magics.ContainsKey(x.ID) ? Source.Magics[x.ID] : 0)) / 100);
                            break;

                        default:
                            Sort = Kernel_bin.MagicData.AsEnumerable();
                            break;
                    }
                }

                public void FillMagic()
                {
                    int pos = 0;
                    int skip = Page * rows;
                    if (Sort != null)
                        foreach (Kernel_bin.Magic_Data i in Sort)
                        {
                            if (pos >= rows) break;
                            if (Source.Magics.ContainsKey(i.ID) && skip-- <= 0 && i.ID > 0)
                            {
                                if (Source.Stat_J.ContainsValue(i.ID))
                                    addMagic(ref pos, i, Font.ColorID.Grey);
                                else
                                    switch (SortMode)
                                    {
                                        case Mode.Mag_Pool_Stat:
                                            if (i.J_Val[Stat] == 0)
                                                addMagic(ref pos, i, Font.ColorID.Dark_Gray);
                                            else
                                                addMagic(ref pos, i, Font.ColorID.White);
                                            break;

                                        case Mode.Mag_Pool_EL_D:
                                            if (i.J_Val[Kernel_bin.Stat.EL_Def_1] * i.Elem_J_def.Count() == 0)
                                                addMagic(ref pos, i, Font.ColorID.Dark_Gray);
                                            else
                                                addMagic(ref pos, i, Font.ColorID.White);
                                            break;

                                        case Mode.Mag_Pool_EL_A:
                                            if (i.J_Val[Kernel_bin.Stat.EL_Atk] * i.Elem_J_atk.Count() == 0)
                                                addMagic(ref pos, i, Font.ColorID.Dark_Gray);
                                            else
                                                addMagic(ref pos, i, Font.ColorID.White);
                                            break;

                                        case Mode.Mag_Pool_ST_D:
                                            if (i.J_Val[Kernel_bin.Stat.ST_Def_1] * i.Stat_J_def.Count() == 0)
                                                addMagic(ref pos, i, Font.ColorID.Dark_Gray);
                                            else
                                                addMagic(ref pos, i, Font.ColorID.White);
                                            break;

                                        case Mode.Mag_Pool_ST_A:
                                            if (i.J_Val[Kernel_bin.Stat.ST_Atk] * i.Stat_J_atk.Count() == 0)
                                                addMagic(ref pos, i, Font.ColorID.Dark_Gray);
                                            else
                                                addMagic(ref pos, i, Font.ColorID.White);
                                            break;

                                        default:
                                            addMagic(ref pos, i, Font.ColorID.White);
                                            break;
                                    }
                            }
                        }
                    for (; pos < rows; pos++)
                    {
                        ITEM[pos, 0] = null;
                        ITEM[pos, 1] = null;
                        ITEM[pos, 2] = null;
                        BLANKS[pos] = true;
                    }
                }

                public override void UpdateTitle()
                {
                    base.UpdateTitle();
                    if (Pages == 1)
                    {
                        ((IGMDataItem_Box)CONTAINER).Title = Icons.ID.MAGIC;
                        ITEM[Count - 1, 0] = ITEM[Count - 2, 0] = null;
                    }
                    else
                        if (Page < Pages)
                        ((IGMDataItem_Box)CONTAINER).Title = (Icons.ID)((int)Icons.ID.MAGIC_PG1 + Page);
                }

                public IGMData Slots { get; private set; } = null;
                public IGMData Values { get; private set; } = null;

                public void Get_Current_Slot_Value()
                {
                    switch (SortMode)
                    {
                        case Mode.Mag_Pool_Stat:
                            Slots = Stat_Slots;
                            Values = null;
                            break;

                        case Mode.Mag_Pool_EL_A:
                            Slots = EL_A_D_Slots;
                            Values = EL_A_Values;
                            break;

                        case Mode.Mag_Pool_EL_D:
                            Slots = EL_A_D_Slots;
                            Values = EL_D_Values;
                            break;

                        case Mode.Mag_Pool_ST_A:
                            Slots = ST_A_D_Slots;
                            Values = ST_A_Values;
                            break;

                        case Mode.Mag_Pool_ST_D:
                            Slots = ST_A_D_Slots;
                            Values = ST_D_Values;
                            break;

                        default:
                            Slots = null;
                            Values = null;
                            break;
                    }
                }

                public void Generate_Preview()
                {
                    if (Slots != null && Stat != Kernel_bin.Stat.None && CURSOR_SELECT < Contents.Length)
                    {
                        Cursor_Status |= Cursor_Status.Enabled;
                        if (Source.Stat_J[Stat] != Contents[CURSOR_SELECT])
                        {
                            Slots.UndoChange();
                            if (Memory.State.Characters != null)
                            {
                                Source = Memory.State.Characters[Character];
                            }
                            if (Source.Stat_J.ContainsValue(Contents[CURSOR_SELECT]))
                            {
                                Kernel_bin.Stat key = Source.Stat_J.FirstOrDefault(x => x.Value == Contents[CURSOR_SELECT]).Key;
                                Source.Stat_J[key] = 0;
                            }
                            Source.Stat_J[Stat] = Contents[CURSOR_SELECT];
                            Slots.ReInit();
                            if (Values != null) Values.ReInit();
                        }
                    }
                }

                public override bool Update()
                {
                    if (InGameMenu_Junction != null &&
                        InGameMenu_Junction.mode != Mode.Mag_Pool_ST_A &&
                        InGameMenu_Junction.mode != Mode.Mag_Pool_ST_D &&
                        InGameMenu_Junction.mode != Mode.Mag_Pool_EL_A &&
                        InGameMenu_Junction.mode != Mode.Mag_Pool_EL_D &&
                        InGameMenu_Junction.mode != Mode.Mag_Pool_Stat)
                    {
                        Cursor_Status &= ~Cursor_Status.Enabled;
                    }
                    if (Memory.State.Characters != null)
                    {
                        Get_Sort_Stat();
                        Get_Slots_Values();
                        if (SortMode != LastMode || this.Stat != LastStat || Character != LastCharacter)
                            Get_Sort();

                        if (!(SortMode == LastMode && Character == LastCharacter && this.Stat == LastStat && Page == LastPage))
                        {
                            LastCharacter = Character;
                            LastStat = this.Stat;
                            LastPage = Page;
                            LastMode = SortMode;
                            FillMagic();
                            UpdateTitle();
                        }
                        if (
                            InGameMenu_Junction.mode == Mode.Mag_Pool_ST_A ||
                            InGameMenu_Junction.mode == Mode.Mag_Pool_ST_D ||
                            InGameMenu_Junction.mode == Mode.Mag_Pool_EL_A ||
                            InGameMenu_Junction.mode == Mode.Mag_Pool_EL_D ||
                            InGameMenu_Junction.mode == Mode.Mag_Pool_Stat)
                        {
                            Get_Current_Slot_Value();
                            Generate_Preview();
                        }
                    }

                    return base.Update();
                }

                protected override void PAGE_PREV()
                {
                    base.PAGE_PREV();
                    ReInit();
                }

                protected override void PAGE_NEXT()
                {
                    base.PAGE_NEXT();
                    ReInit();
                }

                public override void Inputs_CANCEL()
                {
                    if (Memory.State.Characters != null)
                    {
                        base.Inputs_CANCEL();
                        //TODO have pool return to correct screen as there will be 3 possible return modes.
                        if (InGameMenu_Junction.mode == Mode.Mag_Pool_Stat)
                        {
                            InGameMenu_Junction.mode = Mode.Mag_Stat;
                            Stat_Slots.UndoChange();
                            Stat_Slots.ConfirmChange();
                            Stat_Slots.ReInit();
                        }
                        else if (SortMode == Mode.Mag_Pool_EL_A || SortMode == Mode.Mag_Pool_EL_D)
                        {
                            InGameMenu_Junction.mode = Mode.Mag_EL_A;
                            EL_A_D_Slots.UndoChange();
                            EL_A_D_Slots.ConfirmChange();
                            EL_A_D_Slots.ReInit();
                        }
                        else if (SortMode == Mode.Mag_Pool_ST_A || SortMode == Mode.Mag_Pool_ST_D)
                        {
                            InGameMenu_Junction.mode = Mode.Mag_ST_A;
                            ST_A_D_Slots.UndoChange();
                            ST_A_D_Slots.ConfirmChange();
                            ST_A_D_Slots.ReInit();
                        }
                        Cursor_Status &= ~Cursor_Status.Enabled;
                        Source = Memory.State.Characters[Character];
                    }
                }

                public override void Inputs_OKAY()
                {
                    if (Memory.State.Characters != null)
                    {
                        skipsnd = true;
                        init_debugger_Audio.PlaySound(31);
                        base.Inputs_OKAY();
                        if (InGameMenu_Junction.mode == Mode.Mag_Pool_Stat)
                        {
                            InGameMenu_Junction.mode = Mode.Mag_Stat;
                            Stat_Slots.ConfirmChange();
                        }
                        else if (InGameMenu_Junction.mode == Mode.Mag_Pool_EL_A || InGameMenu_Junction.mode == Mode.Mag_Pool_EL_D)
                        {
                            InGameMenu_Junction.mode = Mode.Mag_EL_A;
                            EL_A_D_Slots.ConfirmChange();
                        }
                        else if (InGameMenu_Junction.mode == Mode.Mag_Pool_ST_A || InGameMenu_Junction.mode == Mode.Mag_Pool_ST_D)
                        {
                            InGameMenu_Junction.mode = Mode.Mag_ST_A;
                            ST_A_D_Slots.ConfirmChange();
                        }
                        Cursor_Status &= ~Cursor_Status.Enabled;
                        InGameMenu_Junction.ReInit();
                    }
                }
            }
        }
    }
}