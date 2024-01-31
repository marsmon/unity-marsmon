using Pathfinding.ClipperLib;
using Pathfinding.Poly2Tri;
using Pathfinding.Voxels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.Util
{
	public class TileHandler
	{
		public class TileType
		{
			private VInt3[] verts;

			private int[] tris;

			private VInt3 offset;

			private int lastYOffset;

			private int lastRotation;

			private int width;

			private int depth;

			private static readonly int[] Rotations = new int[]
			{
				1,
				0,
				0,
				1,
				0,
				1,
				-1,
				0,
				-1,
				0,
				0,
				-1,
				0,
				-1,
				1,
				0
			};

			public int Width
			{
				get
				{
					return this.width;
				}
			}

			public int Depth
			{
				get
				{
					return this.depth;
				}
			}

			public TileType(VInt3[] sourceVerts, int[] sourceTris, VInt3 tileSize, VInt3 centerOffset, int width = 1, int depth = 1)
			{
				if (sourceVerts == null)
				{
					throw new ArgumentNullException("sourceVerts");
				}
				if (sourceTris == null)
				{
					throw new ArgumentNullException("sourceTris");
				}
				this.tris = new int[sourceTris.Length];
				for (int i = 0; i < this.tris.Length; i++)
				{
					this.tris[i] = sourceTris[i];
				}
				this.verts = new VInt3[sourceVerts.Length];
				for (int j = 0; j < sourceVerts.Length; j++)
				{
					this.verts[j] = sourceVerts[j] + centerOffset;
				}
				this.offset = tileSize / 2f;
				this.offset.x = this.offset.x * width;
				this.offset.z = this.offset.z * depth;
				this.offset.y = 0;
				for (int k = 0; k < sourceVerts.Length; k++)
				{
					this.verts[k] = this.verts[k] + this.offset;
				}
				this.lastRotation = 0;
				this.lastYOffset = 0;
				this.width = width;
				this.depth = depth;
			}

			public TileType(Mesh source, VInt3 tileSize, VInt3 centerOffset, int width = 1, int depth = 1)
			{
				if (source == null)
				{
					throw new ArgumentNullException("source");
				}
				Vector3[] vertices = source.vertices;
				this.tris = source.triangles;
				this.verts = new VInt3[vertices.Length];
				for (int i = 0; i < vertices.Length; i++)
				{
					this.verts[i] = (VInt3)vertices[i] + centerOffset;
				}
				this.offset = tileSize / 2f;
				this.offset.x = this.offset.x * width;
				this.offset.z = this.offset.z * depth;
				this.offset.y = 0;
				for (int j = 0; j < vertices.Length; j++)
				{
					this.verts[j] = this.verts[j] + this.offset;
				}
				this.lastRotation = 0;
				this.lastYOffset = 0;
				this.width = width;
				this.depth = depth;
			}

			public void Load(out VInt3[] verts, out int[] tris, int rotation, int yoffset)
			{
				rotation = (rotation % 4 + 4) % 4;
				int num = rotation;
				rotation = (rotation - this.lastRotation % 4 + 4) % 4;
				this.lastRotation = num;
				verts = this.verts;
				int num2 = yoffset - this.lastYOffset;
				this.lastYOffset = yoffset;
				if (rotation != 0 || num2 != 0)
				{
					for (int i = 0; i < verts.Length; i++)
					{
						VInt3 vInt = verts[i] - this.offset;
						VInt3 lhs = vInt;
						lhs.y += num2;
						lhs.x = vInt.x * TileHandler.TileType.Rotations[rotation * 4] + vInt.z * TileHandler.TileType.Rotations[rotation * 4 + 1];
						lhs.z = vInt.x * TileHandler.TileType.Rotations[rotation * 4 + 2] + vInt.z * TileHandler.TileType.Rotations[rotation * 4 + 3];
						verts[i] = lhs + this.offset;
					}
				}
				tris = this.tris;
			}
		}

		public enum CutMode
		{
			CutAll = 1,
			CutDual,
			CutExtra = 4
		}

		private const int CUT_ALL = 0;

		private const int CUT_DUAL = 1;

		private const int CUT_BREAK = 2;

		private RecastGraph _graph;

		private ListView<TileHandler.TileType> tileTypes = new ListView<TileHandler.TileType>();

		private Clipper clipper;

		private int[] cached_int_array = new int[32];

		private Dictionary<VInt3, int> cached_Int3_int_dict = new Dictionary<VInt3, int>();

		private Dictionary<VInt2, int> cached_Int2_int_dict = new Dictionary<VInt2, int>();

		private TileHandler.TileType[] activeTileTypes;

		private int[] activeTileRotations;

		private int[] activeTileOffsets;

		private bool[] reloadedInBatch;

		private bool isBatching;

		public RecastGraph graph
		{
			get
			{
				return this._graph;
			}
		}

		public TileHandler(RecastGraph graph)
		{
			if (graph == null)
			{
				throw new ArgumentNullException("'graph' cannot be null");
			}
			if (graph.GetTiles() == null)
			{
				throw new ArgumentException("graph has no tiles. Please scan the graph before creating a TileHandler");
			}
			this.activeTileTypes = new TileHandler.TileType[graph.tileXCount * graph.tileZCount];
			this.activeTileRotations = new int[this.activeTileTypes.Length];
			this.activeTileOffsets = new int[this.activeTileTypes.Length];
			this.reloadedInBatch = new bool[this.activeTileTypes.Length];
			this._graph = graph;
		}

		public int GetActiveRotation(VInt2 p)
		{
			return this.activeTileRotations[p.x + p.y * this._graph.tileXCount];
		}

		public TileHandler.TileType GetTileType(int index)
		{
			return this.tileTypes[index];
		}

		public int GetTileTypeCount()
		{
			return this.tileTypes.Count;
		}

		public TileHandler.TileType RegisterTileType(Mesh source, VInt3 centerOffset, int width = 1, int depth = 1)
		{
			TileHandler.TileType tileType = new TileHandler.TileType(source, new VInt3(this.graph.tileSizeX, 1, this.graph.tileSizeZ) * (1000f * this.graph.cellSize), centerOffset, width, depth);
			this.tileTypes.Add(tileType);
			return tileType;
		}

		public void CreateTileTypesFromGraph()
		{
			RecastGraph.NavmeshTile[] tiles = this.graph.GetTiles();
			if (tiles == null || tiles.Length != this.graph.tileXCount * this.graph.tileZCount)
			{
				throw new InvalidOperationException("Graph tiles are invalid (either null or number of tiles is not equal to width*depth of the graph");
			}
			for (int i = 0; i < this.graph.tileZCount; i++)
			{
				for (int j = 0; j < this.graph.tileXCount; j++)
				{
					RecastGraph.NavmeshTile navmeshTile = tiles[j + i * this.graph.tileXCount];
					VInt3 vInt = (VInt3)this.graph.GetTileBounds(j, i, 1, 1).min;
					VInt3 tileSize = new VInt3(this.graph.tileSizeX, 1, this.graph.tileSizeZ) * (1000f * this.graph.cellSize);
					vInt += new VInt3(tileSize.x * navmeshTile.w / 2, 0, tileSize.z * navmeshTile.d / 2);
					vInt = -vInt;
					TileHandler.TileType tileType = new TileHandler.TileType(navmeshTile.verts, navmeshTile.tris, tileSize, vInt, navmeshTile.w, navmeshTile.d);
					this.tileTypes.Add(tileType);
					int num = j + i * this.graph.tileXCount;
					this.activeTileTypes[num] = tileType;
					this.activeTileRotations[num] = 0;
					this.activeTileOffsets[num] = 0;
				}
			}
		}

		public bool StartBatchLoad()
		{
			if (this.isBatching)
			{
				return false;
			}
			this.isBatching = true;
			AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(delegate(bool force)
			{
				this.graph.StartBatchTileUpdate();
				return true;
			}));
			return true;
		}

		public void EndBatchLoad()
		{
			if (!this.isBatching)
			{
				throw new Exception("Ending batching when batching has not been started");
			}
			for (int i = 0; i < this.reloadedInBatch.Length; i++)
			{
				this.reloadedInBatch[i] = false;
			}
			this.isBatching = false;
			AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(delegate(bool force)
			{
				this.graph.EndBatchTileUpdate();
				return true;
			}));
		}

		private void CutPoly(VInt3[] verts, int[] tris, ref VInt3[] outVertsArr, ref int[] outTrisArr, out int outVCount, out int outTCount, VInt3[] extraShape, VInt3 cuttingOffset, Bounds realBounds, TileHandler.CutMode mode = (TileHandler.CutMode)3, int perturbate = 0)
		{
			if (verts.Length == 0 || tris.Length == 0)
			{
				outVCount = 0;
				outTCount = 0;
				outTrisArr = new int[0];
				outVertsArr = new VInt3[0];
				return;
			}
			List<IntPoint> list = null;
			if (extraShape == null && (mode & TileHandler.CutMode.CutExtra) != (TileHandler.CutMode)0)
			{
				throw new Exception("extraShape is null and the CutMode specifies that it should be used. Cannot use null shape.");
			}
			if ((mode & TileHandler.CutMode.CutExtra) != (TileHandler.CutMode)0)
			{
				list = new List<IntPoint>(extraShape.Length);
				for (int i = 0; i < extraShape.Length; i++)
				{
					list.Add(new IntPoint((long)(extraShape[i].x + cuttingOffset.x), (long)(extraShape[i].z + cuttingOffset.z)));
				}
			}
			List<IntPoint> list2 = new List<IntPoint>(5);
			Dictionary<TriangulationPoint, int> dictionary = new Dictionary<TriangulationPoint, int>();
			List<PolygonPoint> list3 = new List<PolygonPoint>();
			IntRect b = new IntRect(verts[0].x, verts[0].z, verts[0].x, verts[0].z);
			for (int j = 0; j < verts.Length; j++)
			{
				b = b.ExpandToContain(verts[j].x, verts[j].z);
			}
			List<VInt3> list4 = ListPool<VInt3>.Claim(verts.Length * 2);
			List<int> list5 = ListPool<int>.Claim(tris.Length);
			PolyTree polyTree = new PolyTree();
			List<List<IntPoint>> list6 = new List<List<IntPoint>>();
			Stack<Pathfinding.Poly2Tri.Polygon> stack = new Stack<Pathfinding.Poly2Tri.Polygon>();
			if (this.clipper == null)
			{
				this.clipper = new Clipper(0);
			}
			this.clipper.ReverseSolution = true;
			this.clipper.StrictlySimple = true;
			ListView<NavmeshCut> listView;
			if (mode == TileHandler.CutMode.CutExtra)
			{
				listView = new ListView<NavmeshCut>();
			}
			else
			{
				listView = NavmeshCut.GetAllInRange(realBounds);
			}
			List<int> list7 = ListPool<int>.Claim();
			List<IntRect> list8 = ListPool<IntRect>.Claim();
			List<VInt2> list9 = ListPool<VInt2>.Claim();
			List<List<IntPoint>> list10 = new List<List<IntPoint>>();
			List<bool> list11 = ListPool<bool>.Claim();
			List<bool> list12 = ListPool<bool>.Claim();
			if (perturbate > 10)
			{
				Debug.LogError("Too many perturbations aborting : " + mode);
				Debug.Break();
				outVCount = verts.Length;
				outTCount = tris.Length;
				outTrisArr = tris;
				outVertsArr = verts;
				return;
			}
			Random random = null;
			if (perturbate > 0)
			{
				random = new Random();
			}
			for (int k = 0; k < listView.Count; k++)
			{
				Bounds bounds = listView[k].GetBounds();
				VInt3 vInt = (VInt3)bounds.min + cuttingOffset;
				VInt3 vInt2 = (VInt3)bounds.max + cuttingOffset;
				IntRect a = new IntRect(vInt.x, vInt.z, vInt2.x, vInt2.z);
				if (IntRect.Intersects(a, b))
				{
					VInt2 vInt3 = new VInt2(0, 0);
					if (perturbate > 0)
					{
						vInt3.x = random.Next() % 6 * perturbate - 3 * perturbate;
						if (vInt3.x >= 0)
						{
							vInt3.x++;
						}
						vInt3.y = random.Next() % 6 * perturbate - 3 * perturbate;
						if (vInt3.y >= 0)
						{
							vInt3.y++;
						}
					}
					int count = list10.get_Count();
					listView[k].GetContour(list10);
					for (int l = count; l < list10.get_Count(); l++)
					{
						List<IntPoint> list13 = list10.get_Item(l);
						if (list13.get_Count() == 0)
						{
							Debug.LogError("Zero Length Contour");
							list8.Add(default(IntRect));
							list9.Add(new VInt2(0, 0));
						}
						else
						{
							IntRect intRect = new IntRect((int)list13.get_Item(0).X + cuttingOffset.x, (int)list13.get_Item(0).Y + cuttingOffset.y, (int)list13.get_Item(0).X + cuttingOffset.x, (int)list13.get_Item(0).Y + cuttingOffset.y);
							for (int m = 0; m < list13.get_Count(); m++)
							{
								IntPoint intPoint = list13.get_Item(m);
								intPoint.X += (long)cuttingOffset.x;
								intPoint.Y += (long)cuttingOffset.z;
								if (perturbate > 0)
								{
									intPoint.X += (long)vInt3.x;
									intPoint.Y += (long)vInt3.y;
								}
								list13.set_Item(m, intPoint);
								intRect = intRect.ExpandToContain((int)intPoint.X, (int)intPoint.Y);
							}
							list9.Add(new VInt2(vInt.y, vInt2.y));
							list8.Add(intRect);
							list11.Add(listView[k].isDual);
							list12.Add(listView[k].cutsAddedGeom);
						}
					}
				}
			}
			List<NavmeshAdd> allInRange = NavmeshAdd.GetAllInRange(realBounds);
			VInt3[] array = verts;
			int[] array2 = tris;
			int num = -1;
			int n = -3;
			VInt3[] array3 = null;
			VInt3[] array4 = null;
			VInt3 vInt4 = VInt3.zero;
			if (allInRange.get_Count() > 0)
			{
				array3 = new VInt3[7];
				array4 = new VInt3[7];
				vInt4 = (VInt3)realBounds.extents;
			}
			while (true)
			{
				n += 3;
				while (n >= array2.Length)
				{
					num++;
					n = 0;
					if (num >= allInRange.get_Count())
					{
						array = null;
						break;
					}
					if (array == verts)
					{
						array = null;
					}
					allInRange.get_Item(num).GetMesh(cuttingOffset, ref array, out array2);
				}
				if (array == null)
				{
					break;
				}
				VInt3 vInt5 = array[array2[n]];
				VInt3 vInt6 = array[array2[n + 1]];
				VInt3 vInt7 = array[array2[n + 2]];
				IntRect a2 = new IntRect(vInt5.x, vInt5.z, vInt5.x, vInt5.z);
				a2 = a2.ExpandToContain(vInt6.x, vInt6.z);
				a2 = a2.ExpandToContain(vInt7.x, vInt7.z);
				int num2 = Math.Min(vInt5.y, Math.Min(vInt6.y, vInt7.y));
				int num3 = Math.Max(vInt5.y, Math.Max(vInt6.y, vInt7.y));
				list7.Clear();
				bool flag = false;
				for (int num4 = 0; num4 < list10.get_Count(); num4++)
				{
					int x = list9.get_Item(num4).x;
					int y = list9.get_Item(num4).y;
					if (IntRect.Intersects(a2, list8.get_Item(num4)) && y >= num2 && x <= num3 && (list12.get_Item(num4) || num == -1))
					{
						VInt3 vInt8 = vInt5;
						vInt8.y = x;
						VInt3 vInt9 = vInt5;
						vInt9.y = y;
						list7.Add(num4);
						flag |= list11.get_Item(num4);
					}
				}
				if (list7.get_Count() == 0 && (mode & TileHandler.CutMode.CutExtra) == (TileHandler.CutMode)0 && (mode & TileHandler.CutMode.CutAll) != (TileHandler.CutMode)0 && num == -1)
				{
					list5.Add(list4.get_Count());
					list5.Add(list4.get_Count() + 1);
					list5.Add(list4.get_Count() + 2);
					list4.Add(vInt5);
					list4.Add(vInt6);
					list4.Add(vInt7);
				}
				else
				{
					list2.Clear();
					if (num == -1)
					{
						list2.Add(new IntPoint((long)vInt5.x, (long)vInt5.z));
						list2.Add(new IntPoint((long)vInt6.x, (long)vInt6.z));
						list2.Add(new IntPoint((long)vInt7.x, (long)vInt7.z));
					}
					else
					{
						array3[0] = vInt5;
						array3[1] = vInt6;
						array3[2] = vInt7;
						int num5 = Utility.ClipPolygon(array3, 3, array4, 1, 0, 0);
						if (num5 == 0)
						{
							continue;
						}
						num5 = Utility.ClipPolygon(array4, num5, array3, -1, 2 * vInt4.x, 0);
						if (num5 == 0)
						{
							continue;
						}
						num5 = Utility.ClipPolygon(array3, num5, array4, 1, 0, 2);
						if (num5 == 0)
						{
							continue;
						}
						num5 = Utility.ClipPolygon(array4, num5, array3, -1, 2 * vInt4.z, 2);
						if (num5 == 0)
						{
							continue;
						}
						for (int num6 = 0; num6 < num5; num6++)
						{
							list2.Add(new IntPoint((long)array3[num6].x, (long)array3[num6].z));
						}
					}
					dictionary.Clear();
					VInt3 vInt10 = vInt6 - vInt5;
					VInt3 vInt11 = vInt7 - vInt5;
					VInt3 vInt12 = vInt10;
					VInt3 vInt13 = vInt11;
					vInt12.y = 0;
					vInt13.y = 0;
					for (int num7 = 0; num7 < 16; num7++)
					{
						if ((mode >> (num7 & 31) & TileHandler.CutMode.CutAll) != (TileHandler.CutMode)0)
						{
							if (1 << num7 == 1)
							{
								this.clipper.Clear();
								this.clipper.AddPolygon(list2, PolyType.ptSubject);
								for (int num8 = 0; num8 < list7.get_Count(); num8++)
								{
									this.clipper.AddPolygon(list10.get_Item(list7.get_Item(num8)), PolyType.ptClip);
								}
								polyTree.Clear();
								this.clipper.Execute(ClipType.ctDifference, polyTree, PolyFillType.pftEvenOdd, PolyFillType.pftNonZero);
							}
							else if (1 << num7 == 2)
							{
								if (!flag)
								{
									goto IL_1170;
								}
								this.clipper.Clear();
								this.clipper.AddPolygon(list2, PolyType.ptSubject);
								for (int num9 = 0; num9 < list7.get_Count(); num9++)
								{
									if (list11.get_Item(list7.get_Item(num9)))
									{
										this.clipper.AddPolygon(list10.get_Item(list7.get_Item(num9)), PolyType.ptClip);
									}
								}
								list6.Clear();
								this.clipper.Execute(ClipType.ctIntersection, list6, PolyFillType.pftEvenOdd, PolyFillType.pftNonZero);
								this.clipper.Clear();
								for (int num10 = 0; num10 < list6.get_Count(); num10++)
								{
									this.clipper.AddPolygon(list6.get_Item(num10), (!Clipper.Orientation(list6.get_Item(num10))) ? PolyType.ptSubject : PolyType.ptClip);
								}
								for (int num11 = 0; num11 < list7.get_Count(); num11++)
								{
									if (!list11.get_Item(list7.get_Item(num11)))
									{
										this.clipper.AddPolygon(list10.get_Item(list7.get_Item(num11)), PolyType.ptClip);
									}
								}
								polyTree.Clear();
								this.clipper.Execute(ClipType.ctDifference, polyTree, PolyFillType.pftEvenOdd, PolyFillType.pftNonZero);
							}
							else if (1 << num7 == 4)
							{
								this.clipper.Clear();
								this.clipper.AddPolygon(list2, PolyType.ptSubject);
								this.clipper.AddPolygon(list, PolyType.ptClip);
								polyTree.Clear();
								this.clipper.Execute(ClipType.ctIntersection, polyTree, PolyFillType.pftEvenOdd, PolyFillType.pftNonZero);
							}
							for (int num12 = 0; num12 < polyTree.ChildCount; num12++)
							{
								PolyNode polyNode = polyTree.Childs.get_Item(num12);
								List<IntPoint> contour = polyNode.Contour;
								List<PolyNode> childs = polyNode.Childs;
								if (childs.get_Count() == 0 && contour.get_Count() == 3 && num == -1)
								{
									for (int num13 = 0; num13 < contour.get_Count(); num13++)
									{
										VInt3 vInt14 = new VInt3((int)contour.get_Item(num13).X, 0, (int)contour.get_Item(num13).Y);
										double num14 = (double)(vInt6.z - vInt7.z) * (double)(vInt5.x - vInt7.x) + (double)(vInt7.x - vInt6.x) * (double)(vInt5.z - vInt7.z);
										if (num14 == 0.0)
										{
											Debug.LogWarning("Degenerate triangle");
										}
										else
										{
											double num15 = ((double)(vInt6.z - vInt7.z) * (double)(vInt14.x - vInt7.x) + (double)(vInt7.x - vInt6.x) * (double)(vInt14.z - vInt7.z)) / num14;
											double num16 = ((double)(vInt7.z - vInt5.z) * (double)(vInt14.x - vInt7.x) + (double)(vInt5.x - vInt7.x) * (double)(vInt14.z - vInt7.z)) / num14;
											vInt14.y = MMGame_Math.RoundToInt(num15 * (double)vInt5.y + num16 * (double)vInt6.y + (1.0 - num15 - num16) * (double)vInt7.y);
											list5.Add(list4.get_Count());
											list4.Add(vInt14);
										}
									}
								}
								else
								{
									Pathfinding.Poly2Tri.Polygon polygon = null;
									int num17 = -1;
									for (List<IntPoint> list14 = contour; list14 != null; list14 = ((num17 >= childs.get_Count()) ? null : childs.get_Item(num17).Contour))
									{
										list3.Clear();
										for (int num18 = 0; num18 < list14.get_Count(); num18++)
										{
											PolygonPoint polygonPoint = new PolygonPoint((double)list14.get_Item(num18).X, (double)list14.get_Item(num18).Y);
											list3.Add(polygonPoint);
											VInt3 vInt15 = new VInt3((int)list14.get_Item(num18).X, 0, (int)list14.get_Item(num18).Y);
											double num19 = (double)(vInt6.z - vInt7.z) * (double)(vInt5.x - vInt7.x) + (double)(vInt7.x - vInt6.x) * (double)(vInt5.z - vInt7.z);
											if (num19 == 0.0)
											{
												Debug.LogWarning("Degenerate triangle");
											}
											else
											{
												double num20 = ((double)(vInt6.z - vInt7.z) * (double)(vInt15.x - vInt7.x) + (double)(vInt7.x - vInt6.x) * (double)(vInt15.z - vInt7.z)) / num19;
												double num21 = ((double)(vInt7.z - vInt5.z) * (double)(vInt15.x - vInt7.x) + (double)(vInt5.x - vInt7.x) * (double)(vInt15.z - vInt7.z)) / num19;
												vInt15.y = MMGame_Math.RoundToInt(num20 * (double)vInt5.y + num21 * (double)vInt6.y + (1.0 - num20 - num21) * (double)vInt7.y);
												dictionary.set_Item(polygonPoint, list4.get_Count());
												list4.Add(vInt15);
											}
										}
										Pathfinding.Poly2Tri.Polygon polygon2;
										if (stack.Count > 0)
										{
											polygon2 = stack.Pop();
											polygon2.AddPoints(list3);
										}
										else
										{
											polygon2 = new Pathfinding.Poly2Tri.Polygon(list3);
										}
										if (polygon == null)
										{
											polygon = polygon2;
										}
										else
										{
											polygon.AddHole(polygon2);
										}
										num17++;
									}
									try
									{
										P2T.Triangulate(polygon);
									}
									catch (PointOnEdgeException)
									{
										Debug.LogWarning(string.Concat(new object[]
										{
											"PointOnEdgeException, perturbating vertices slightly ( at ",
											num7,
											" in ",
											mode,
											")"
										}));
										this.CutPoly(verts, tris, ref outVertsArr, ref outTrisArr, out outVCount, out outTCount, extraShape, cuttingOffset, realBounds, mode, perturbate + 1);
										return;
									}
									for (int num22 = 0; num22 < polygon.Triangles.get_Count(); num22++)
									{
										DelaunayTriangle delaunayTriangle = polygon.Triangles.get_Item(num22);
										list5.Add(dictionary.get_Item(delaunayTriangle.Points._0));
										list5.Add(dictionary.get_Item(delaunayTriangle.Points._1));
										list5.Add(dictionary.get_Item(delaunayTriangle.Points._2));
									}
									if (polygon.Holes != null)
									{
										for (int num23 = 0; num23 < polygon.Holes.get_Count(); num23++)
										{
											polygon.Holes.get_Item(num23).Points.Clear();
											polygon.Holes.get_Item(num23).ClearTriangles();
											if (polygon.Holes.get_Item(num23).Holes != null)
											{
												polygon.Holes.get_Item(num23).Holes.Clear();
											}
											stack.Push(polygon.Holes.get_Item(num23));
										}
									}
									polygon.ClearTriangles();
									if (polygon.Holes != null)
									{
										polygon.Holes.Clear();
									}
									polygon.Points.Clear();
									stack.Push(polygon);
								}
							}
						}
						IL_1170:;
					}
				}
			}
			Dictionary<VInt3, int> dictionary2 = this.cached_Int3_int_dict;
			dictionary2.Clear();
			if (this.cached_int_array.Length < list4.get_Count())
			{
				this.cached_int_array = new int[Math.Max(this.cached_int_array.Length * 2, list4.get_Count())];
			}
			int[] array5 = this.cached_int_array;
			int num24 = 0;
			for (int num25 = 0; num25 < list4.get_Count(); num25++)
			{
				int num26;
				if (!dictionary2.TryGetValue(list4.get_Item(num25), ref num26))
				{
					dictionary2.Add(list4.get_Item(num25), num24);
					array5[num25] = num24;
					list4.set_Item(num24, list4.get_Item(num25));
					num24++;
				}
				else
				{
					array5[num25] = num26;
				}
			}
			outTCount = list5.get_Count();
			if (outTrisArr == null || outTrisArr.Length < outTCount)
			{
				outTrisArr = new int[outTCount];
			}
			for (int num27 = 0; num27 < outTCount; num27++)
			{
				outTrisArr[num27] = array5[list5.get_Item(num27)];
			}
			outVCount = num24;
			if (outVertsArr == null || outVertsArr.Length < outVCount)
			{
				outVertsArr = new VInt3[outVCount];
			}
			for (int num28 = 0; num28 < outVCount; num28++)
			{
				outVertsArr[num28] = list4.get_Item(num28);
			}
			for (int num29 = 0; num29 < listView.Count; num29++)
			{
				listView[num29].UsedForCut();
			}
			ListPool<VInt3>.Release(list4);
			ListPool<int>.Release(list5);
			ListPool<int>.Release(list7);
			ListPool<VInt2>.Release(list9);
			ListPool<bool>.Release(list11);
			ListPool<bool>.Release(list12);
			ListPool<IntRect>.Release(list8);
		}

		private void DelaunayRefinement(VInt3[] verts, int[] tris, ref int vCount, ref int tCount, bool delaunay, bool colinear, VInt3 worldOffset)
		{
			if (tCount % 3 != 0)
			{
				throw new Exception("Triangle array length must be a multiple of 3");
			}
			Dictionary<VInt2, int> dictionary = this.cached_Int2_int_dict;
			dictionary.Clear();
			for (int i = 0; i < tCount; i += 3)
			{
				if (!Pathfinding.Polygon.IsClockwise(verts[tris[i]], verts[tris[i + 1]], verts[tris[i + 2]]))
				{
					int num = tris[i];
					tris[i] = tris[i + 2];
					tris[i + 2] = num;
				}
				dictionary.set_Item(new VInt2(tris[i], tris[i + 1]), i + 2);
				dictionary.set_Item(new VInt2(tris[i + 1], tris[i + 2]), i);
				dictionary.set_Item(new VInt2(tris[i + 2], tris[i]), i + 1);
			}
			int num2 = 9;
			for (int j = 0; j < tCount; j += 3)
			{
				for (int k = 0; k < 3; k++)
				{
					int num3;
					if (dictionary.TryGetValue(new VInt2(tris[j + (k + 1) % 3], tris[j + k % 3]), ref num3))
					{
						VInt3 vInt = verts[tris[j + (k + 2) % 3]];
						VInt3 vInt2 = verts[tris[j + (k + 1) % 3]];
						VInt3 vInt3 = verts[tris[j + (k + 3) % 3]];
						VInt3 vInt4 = verts[tris[num3]];
						vInt.y = 0;
						vInt2.y = 0;
						vInt3.y = 0;
						vInt4.y = 0;
						bool flag = false;
						if (!Pathfinding.Polygon.Left(vInt, vInt3, vInt4) || Pathfinding.Polygon.LeftNotColinear(vInt, vInt2, vInt4))
						{
							if (!colinear)
							{
								goto IL_439;
							}
							flag = true;
						}
						if (colinear && AstarMath.DistancePointSegment(vInt, vInt4, vInt2) < (float)num2 && !dictionary.ContainsKey(new VInt2(tris[j + (k + 2) % 3], tris[j + (k + 1) % 3])) && !dictionary.ContainsKey(new VInt2(tris[j + (k + 1) % 3], tris[num3])))
						{
							tCount -= 3;
							int num4 = num3 / 3 * 3;
							tris[j + (k + 1) % 3] = tris[num3];
							if (num4 != tCount)
							{
								tris[num4] = tris[tCount];
								tris[num4 + 1] = tris[tCount + 1];
								tris[num4 + 2] = tris[tCount + 2];
								dictionary.set_Item(new VInt2(tris[num4], tris[num4 + 1]), num4 + 2);
								dictionary.set_Item(new VInt2(tris[num4 + 1], tris[num4 + 2]), num4);
								dictionary.set_Item(new VInt2(tris[num4 + 2], tris[num4]), num4 + 1);
								tris[tCount] = 0;
								tris[tCount + 1] = 0;
								tris[tCount + 2] = 0;
							}
							else
							{
								tCount += 3;
							}
							dictionary.set_Item(new VInt2(tris[j], tris[j + 1]), j + 2);
							dictionary.set_Item(new VInt2(tris[j + 1], tris[j + 2]), j);
							dictionary.set_Item(new VInt2(tris[j + 2], tris[j]), j + 1);
						}
						else if (delaunay && !flag)
						{
							float num5 = VInt3.Angle(vInt2 - vInt, vInt3 - vInt);
							float num6 = VInt3.Angle(vInt2 - vInt4, vInt3 - vInt4);
							if (num6 > 6.28318548f - 2f * num5)
							{
								tris[j + (k + 1) % 3] = tris[num3];
								int num7 = num3 / 3 * 3;
								int num8 = num3 - num7;
								tris[num7 + (num8 - 1 + 3) % 3] = tris[j + (k + 2) % 3];
								dictionary.set_Item(new VInt2(tris[j], tris[j + 1]), j + 2);
								dictionary.set_Item(new VInt2(tris[j + 1], tris[j + 2]), j);
								dictionary.set_Item(new VInt2(tris[j + 2], tris[j]), j + 1);
								dictionary.set_Item(new VInt2(tris[num7], tris[num7 + 1]), num7 + 2);
								dictionary.set_Item(new VInt2(tris[num7 + 1], tris[num7 + 2]), num7);
								dictionary.set_Item(new VInt2(tris[num7 + 2], tris[num7]), num7 + 1);
							}
						}
					}
					IL_439:;
				}
			}
		}

		private Vector3 Point2D2V3(TriangulationPoint p)
		{
			return new Vector3((float)p.X, 0f, (float)p.Y) * 0.001f;
		}

		private VInt3 IntPoint2Int3(IntPoint p)
		{
			return new VInt3((int)p.X, 0, (int)p.Y);
		}

		public void ClearTile(int x, int z)
		{
			if (AstarPath.active == null)
			{
				return;
			}
			if (x < 0 || z < 0 || x >= this.graph.tileXCount || z >= this.graph.tileZCount)
			{
				return;
			}
			AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(delegate(bool force)
			{
				this.graph.ReplaceTile(x, z, new VInt3[0], new int[0], false);
				this.activeTileTypes[x + z * this.graph.tileXCount] = null;
				GraphModifier.TriggerEvent(GraphModifier.EventType.PostUpdate);
				AstarPath.active.QueueWorkItemFloodFill();
				return true;
			}));
		}

		public void ReloadInBounds(Bounds b)
		{
			VInt2 tileCoordinates = this.graph.GetTileCoordinates(b.min);
			VInt2 tileCoordinates2 = this.graph.GetTileCoordinates(b.max);
			IntRect a = new IntRect(tileCoordinates.x, tileCoordinates.y, tileCoordinates2.x, tileCoordinates2.y);
			a = IntRect.Intersection(a, new IntRect(0, 0, this.graph.tileXCount - 1, this.graph.tileZCount - 1));
			if (!a.IsValid())
			{
				return;
			}
			for (int i = a.ymin; i <= a.ymax; i++)
			{
				for (int j = a.xmin; j <= a.xmax; j++)
				{
					this.ReloadTile(j, i);
				}
			}
		}

		public void ReloadTile(int x, int z)
		{
			if (x < 0 || z < 0 || x >= this.graph.tileXCount || z >= this.graph.tileZCount)
			{
				return;
			}
			int num = x + z * this.graph.tileXCount;
			if (this.activeTileTypes[num] != null)
			{
				this.LoadTile(this.activeTileTypes[num], x, z, this.activeTileRotations[num], this.activeTileOffsets[num]);
			}
		}

		public void ReloadTiles()
		{
			for (int i = 0; i < this.graph.tileXCount; i++)
			{
				for (int j = 0; j < this.graph.tileZCount; j++)
				{
					int num = i + j * this.graph.tileXCount;
					if (this.activeTileTypes[num] != null)
					{
						this.LoadTile(this.activeTileTypes[num], i, j, this.activeTileRotations[num], this.activeTileOffsets[num]);
					}
				}
			}
		}

		public void CutShapeWithTile(int x, int z, VInt3[] shape, ref VInt3[] verts, ref int[] tris, out int vCount, out int tCount)
		{
			if (this.isBatching)
			{
				throw new Exception("Cannot cut with shape when batching. Please stop batching first.");
			}
			int num = x + z * this.graph.tileXCount;
			if (x < 0 || z < 0 || x >= this.graph.tileXCount || z >= this.graph.tileZCount || this.activeTileTypes[num] == null)
			{
				verts = new VInt3[0];
				tris = new int[0];
				vCount = 0;
				tCount = 0;
				return;
			}
			VInt3[] verts2;
			int[] tris2;
			this.activeTileTypes[num].Load(out verts2, out tris2, this.activeTileRotations[num], this.activeTileOffsets[num]);
			Bounds tileBounds = this.graph.GetTileBounds(x, z, 1, 1);
			VInt3 vInt = (VInt3)tileBounds.min;
			vInt = -vInt;
			this.CutPoly(verts2, tris2, ref verts, ref tris, out vCount, out tCount, shape, vInt, tileBounds, TileHandler.CutMode.CutExtra, 0);
			for (int i = 0; i < verts.Length; i++)
			{
				verts[i] -= vInt;
			}
		}

		protected static T[] ShrinkArray<T>(T[] arr, int newLength)
		{
			newLength = Math.Min(newLength, arr.Length);
			T[] array = new T[newLength];
			if (newLength % 4 == 0)
			{
				for (int i = 0; i < newLength; i += 4)
				{
					array[i] = arr[i];
					array[i + 1] = arr[i + 1];
					array[i + 2] = arr[i + 2];
					array[i + 3] = arr[i + 3];
				}
			}
			else if (newLength % 3 == 0)
			{
				for (int j = 0; j < newLength; j += 3)
				{
					array[j] = arr[j];
					array[j + 1] = arr[j + 1];
					array[j + 2] = arr[j + 2];
				}
			}
			else if (newLength % 2 == 0)
			{
				for (int k = 0; k < newLength; k += 2)
				{
					array[k] = arr[k];
					array[k + 1] = arr[k + 1];
				}
			}
			else
			{
				for (int l = 0; l < newLength; l++)
				{
					array[l] = arr[l];
				}
			}
			return array;
		}

		public void LoadTile(TileHandler.TileType tile, int x, int z, int rotation, int yoffset)
		{
			if (tile == null)
			{
				throw new ArgumentNullException("tile");
			}
			if (AstarPath.active == null)
			{
				return;
			}
			int index = x + z * this.graph.tileXCount;
			rotation %= 4;
			if (this.isBatching && this.reloadedInBatch[index] && this.activeTileOffsets[index] == yoffset && this.activeTileRotations[index] == rotation && this.activeTileTypes[index] == tile)
			{
				return;
			}
			if (this.isBatching)
			{
				this.reloadedInBatch[index] = true;
			}
			this.activeTileOffsets[index] = yoffset;
			this.activeTileRotations[index] = rotation;
			this.activeTileTypes[index] = tile;
			AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(delegate(bool force)
			{
				if (this.activeTileOffsets[index] != yoffset || this.activeTileRotations[index] != rotation || this.activeTileTypes[index] != tile)
				{
					return true;
				}
				GraphModifier.TriggerEvent(GraphModifier.EventType.PreUpdate);
				VInt3[] verts;
				int[] tris;
				tile.Load(out verts, out tris, rotation, yoffset);
				Bounds tileBounds = this.graph.GetTileBounds(x, z, tile.Width, tile.Depth);
				VInt3 vInt = (VInt3)tileBounds.min;
				vInt = -vInt;
				VInt3[] array = null;
				int[] array2 = null;
				int num;
				int num2;
				this.CutPoly(verts, tris, ref array, ref array2, out num, out num2, null, vInt, tileBounds, (TileHandler.CutMode)3, 0);
				this.DelaunayRefinement(array, array2, ref num, ref num2, true, false, -vInt);
				if (num2 != array2.Length)
				{
					array2 = TileHandler.ShrinkArray<int>(array2, num2);
				}
				if (num != array.Length)
				{
					array = TileHandler.ShrinkArray<VInt3>(array, num);
				}
				int w = (rotation % 2 != 0) ? tile.Depth : tile.Width;
				int d = (rotation % 2 != 0) ? tile.Width : tile.Depth;
				this.graph.ReplaceTile(x, z, w, d, array, array2, false);
				GraphModifier.TriggerEvent(GraphModifier.EventType.PostUpdate);
				AstarPath.active.QueueWorkItemFloodFill();
				return true;
			}));
		}
	}
}
