using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUIPolygon : Graphic
{
	public Vector3 deltaCenterV3 = default(Vector3);

	public Vector3[] vertexs = new Vector3[]
	{
		new Vector3(0f, 100f),
		new Vector3(-50f, 0f),
		new Vector3(50f, 0f)
	};

	protected override void Start()
	{
		base.transform.localPosition = Vector3.zero;
		base.Start();
	}

	protected override void OnFillVBO(List<UIVertex> vbo)
	{
		if (this.vertexs == null || this.vertexs.Length < 3)
		{
			return;
		}
		this.UpdateVBO(vbo);
	}

	private void UpdateVBO(List<UIVertex> vbo)
	{
		if (this.vertexs.Length == 3)
		{
			UIVertex simpleVert;
			for (int i = 0; i < this.vertexs.Length; i++)
			{
				simpleVert = UIVertex.simpleVert;
				simpleVert.color = base.color;
				simpleVert.position = this.vertexs[i];
				vbo.Add(simpleVert);
			}
			simpleVert = UIVertex.simpleVert;
			simpleVert.color = base.color;
			simpleVert.position = this.vertexs[0];
			vbo.Add(simpleVert);
		}
		else if (this.vertexs.Length == 4)
		{
			for (int j = 0; j < this.vertexs.Length; j++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = base.color;
				simpleVert.position = this.vertexs[j];
				vbo.Add(simpleVert);
			}
		}
		else
		{
			for (int k = 0; k < this.vertexs.Length; k++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = base.color;
				simpleVert.position = this.vertexs[k];
				vbo.Add(simpleVert);
				if (k != this.vertexs.Length - 1)
				{
					simpleVert = UIVertex.simpleVert;
					simpleVert.color = base.color;
					simpleVert.position = this.vertexs[k + 1];
					vbo.Add(simpleVert);
				}
				else
				{
					simpleVert = UIVertex.simpleVert;
					simpleVert.color = base.color;
					simpleVert.position = this.vertexs[0];
					vbo.Add(simpleVert);
				}
				simpleVert = UIVertex.simpleVert;
				simpleVert.color = base.color;
				simpleVert.position = this.deltaCenterV3;
				vbo.Add(simpleVert);
				simpleVert = UIVertex.simpleVert;
				simpleVert.color = base.color;
				simpleVert.position = this.deltaCenterV3;
				vbo.Add(simpleVert);
			}
		}
	}
}
