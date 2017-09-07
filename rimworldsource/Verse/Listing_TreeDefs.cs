using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace Verse
{
	public class Listing_TreeDefs : Listing_Tree
	{
		public Listing_TreeDefs(Rect rect, float labelColumnWidth) : base(rect, labelColumnWidth)
		{
		}
		public void DoContentLines(TreeNode_Editor node, int indentLevel)
		{
			node.DoSpecialPreElements(this);
			if (node.children == null)
			{
				Log.Error(node + " children is null.");
				return;
			}
			for (int i = 0; i < node.children.Count; i++)
			{
				this.DoNode((TreeNode_Editor)node.children[i], indentLevel, 64);
			}
		}
		private void DoNode(TreeNode_Editor node, int indentLevel, int openMask)
		{
			if (node.nodeType == EditTreeNodeType.TerminalValue)
			{
				node.DoSpecialPreElements(this);
				base.DoOpenCloseWidget(node, indentLevel, openMask);
				this.DrawNodeLabelLeft(node, indentLevel);
				WidgetRow widgetRow = new WidgetRow(this.labelWidth, this.curY, UIDirection.RightThenUp, 2000f, 29f);
				this.DoControlButtonsRight(node, widgetRow);
				this.DoValueEditWidgetRight(node, widgetRow.FinalX);
				base.EndLine();
			}
			else
			{
				base.DoOpenCloseWidget(node, indentLevel, openMask);
				this.DrawNodeLabelLeft(node, indentLevel);
				WidgetRow widgetRow2 = new WidgetRow(this.labelWidth, this.curY, UIDirection.RightThenUp, 2000f, 29f);
				this.DoControlButtonsRight(node, widgetRow2);
				this.DrawExtraInfoText(node, widgetRow2);
				base.EndLine();
				if (node.IsOpen(openMask))
				{
					this.DoContentLines(node, indentLevel + 1);
				}
				if (node.nodeType == EditTreeNodeType.ListRoot)
				{
					node.CheckLatentDelete();
				}
			}
		}
		private void DoControlButtonsRight(TreeNode_Editor node, WidgetRow widgetRow)
		{
			if (node.HasNewButton && widgetRow.DoIconButton(TexButton.NewItem, null))
			{
				Action<object> addAction = delegate(object o)
				{
					node.owningField.SetValue(node.ParentObj, o);
					((TreeNode_Editor)node.parentNode).RebuildChildNodes();
				};
				this.MakeCreateNewObjectMenu(node, node.owningField, node.owningField.FieldType, addAction);
			}
			if (node.nodeType == EditTreeNodeType.ListRoot && widgetRow.DoIconButton(TexButton.Add, null))
			{
				Type baseType = node.obj.GetType().GetGenericArguments()[0];
				Action<object> addAction2 = delegate(object o)
				{
					node.obj.GetType().GetMethod("Add").Invoke(node.obj, new object[]
					{
						o
					});
				};
				this.MakeCreateNewObjectMenu(node, node.owningField, baseType, addAction2);
			}
			if (node.HasDeleteButton && widgetRow.DoIconButton(TexButton.DeleteX, null))
			{
				node.Delete();
			}
		}
		private void DrawExtraInfoText(TreeNode_Editor node, WidgetRow widgetRow)
		{
			string extraInfoText = node.ExtraInfoText;
			if (extraInfoText != string.Empty)
			{
				if (extraInfoText == "null")
				{
					GUI.color = new Color(1f, 0.6f, 0.6f, 0.5f);
				}
				else
				{
					GUI.color = new Color(1f, 1f, 1f, 0.5f);
				}
				widgetRow.DoLabel(extraInfoText, -1f);
				GUI.color = Color.white;
			}
		}
		protected void DrawNodeLabelLeft(TreeNode_Editor node, int indentLevel)
		{
			string tipText = string.Empty;
			if (node.owningField != null)
			{
				DescriptionAttribute[] array = (DescriptionAttribute[])node.owningField.GetCustomAttributes(typeof(DescriptionAttribute), true);
				if (array.Length > 0)
				{
					tipText = array[0].description;
				}
			}
			base.DrawLabelLeft(node.LabelText, tipText, indentLevel);
		}
		protected void MakeCreateNewObjectMenu(TreeNode_Editor owningNode, FieldInfo owningField, Type baseType, Action<object> addAction)
		{
			List<Type> list = baseType.InstantiableDescendantsAndSelf().ToList<Type>();
			List<FloatMenuOption> list2 = new List<FloatMenuOption>();
			foreach (Type current in list)
			{
				Type creatingType = current;
				Action action = delegate
				{
					owningNode.SetOpen(-1, true);
					object obj;
					if (creatingType == typeof(string))
					{
						obj = string.Empty;
					}
					else
					{
						obj = Activator.CreateInstance(creatingType);
					}
					addAction(obj);
					if (owningNode != null)
					{
						owningNode.RebuildChildNodes();
					}
				};
				list2.Add(new FloatMenuOption(current.ToString(), action, MenuOptionPriority.Medium, null, null));
			}
			Find.WindowStack.Add(new FloatMenu(list2, false));
		}
		protected void DoValueEditWidgetRight(TreeNode_Editor node, float leftX)
		{
			if (node.nodeType != EditTreeNodeType.TerminalValue)
			{
				throw new ArgumentException();
			}
			Rect rect = new Rect(leftX, this.curY, base.WholeColumnWidth - leftX, this.lineHeight);
			object obj = node.Value;
			Type objectType = node.ObjectType;
			if (objectType == typeof(string))
			{
				string text = (string)obj;
				string text2 = text;
				if (text2 == null)
				{
					text2 = string.Empty;
				}
				string b = text2;
				text2 = Widgets.TextField(rect, text2);
				if (text2 != b)
				{
					text = text2;
				}
				obj = text;
			}
			else
			{
				if (objectType == typeof(bool))
				{
					bool flag = (bool)obj;
					Widgets.Checkbox(new Vector2(rect.x, rect.y), ref flag, this.lineHeight, false);
					obj = flag;
				}
				else
				{
					if (objectType == typeof(int))
					{
						rect.width = 100f;
						string s = Widgets.TextField(rect, obj.ToString());
						int num;
						if (int.TryParse(s, out num))
						{
							obj = num;
						}
					}
					else
					{
						if (objectType == typeof(float))
						{
							EditSliderRangeAttribute[] array = (EditSliderRangeAttribute[])node.owningField.GetCustomAttributes(typeof(EditSliderRangeAttribute), true);
							if (array.Length > 0)
							{
								float num2 = (float)obj;
								Rect position = new Rect(this.labelWidth + 60f + 4f, this.curY, this.editAreaWidth - 60f - 8f, this.lineHeight);
								num2 = GUI.HorizontalSlider(position, num2, array[0].min, array[0].max);
								obj = num2;
							}
							rect.width = 60f;
							string text3 = obj.ToString();
							text3 = Widgets.TextField(rect, text3);
							float num3;
							if (float.TryParse(text3, out num3))
							{
								obj = num3;
							}
						}
						else
						{
							if (objectType.IsEnum)
							{
								if (Widgets.TextButton(rect, obj.ToString(), true, false))
								{
									List<FloatMenuOption> list = new List<FloatMenuOption>();
									IEnumerator enumerator = Enum.GetValues(objectType).GetEnumerator();
									try
									{
										while (enumerator.MoveNext())
										{
											object current = enumerator.Current;
											object localVal = current;
											list.Add(new FloatMenuOption(current.ToString(), delegate
											{
												node.Value = localVal;
											}, MenuOptionPriority.Medium, null, null));
										}
									}
									finally
									{
										IDisposable disposable = enumerator as IDisposable;
										if (disposable != null)
										{
											disposable.Dispose();
										}
									}
									Find.WindowStack.Add(new FloatMenu(list, false));
								}
							}
							else
							{
								if (objectType == typeof(FloatRange))
								{
									float sliderMin = 0f;
									float sliderMax = 100f;
									EditSliderRangeAttribute[] array2 = (EditSliderRangeAttribute[])node.owningField.GetCustomAttributes(typeof(EditSliderRangeAttribute), true);
									if (array2.Length > 0)
									{
										sliderMin = array2[0].min;
										sliderMax = array2[0].max;
									}
									FloatRange floatRange = (FloatRange)obj;
									Widgets.FloatRangeWithTypeIn(rect, node.owningIndex, ref floatRange, sliderMin, sliderMax, ToStringStyle.FloatTwo, null);
									obj = floatRange;
								}
								else
								{
									GUI.color = new Color(1f, 1f, 1f, 0.4f);
									Widgets.Label(rect, "uneditable value type");
									GUI.color = Color.white;
								}
							}
						}
					}
				}
			}
			node.Value = obj;
		}
	}
}
