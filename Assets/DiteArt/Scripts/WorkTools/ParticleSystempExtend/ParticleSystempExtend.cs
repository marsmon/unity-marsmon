using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YF.Art
{

    public enum psExAxis
    {
        none,
        x,
        y,
        z,
        xy,
        xz,
        yz,
        xyz
    }
    [ExecuteInEditMode]
    public class ParticleSystempExtend : MonoBehaviour
    {
        public psExAxis StopAxis = psExAxis.none;
        public bool isWorld = false;
        public Bounds fanwei = new Bounds(Vector3.zero, Vector3.one);
        public bool IsDie = false;

        [HideInInspector] public ParticleSystem m_System;
        [HideInInspector] public ParticleSystem.Particle[] m_Particles;//这里可以优化一下存储的数量
        Vector4[] PosCache;//这里可以优化一下存储的数量

        void Start()
        {
            InitializeIfNeeded();
        }

        void LateUpdate()
        {
            InitializeIfNeeded();
            int numParticlesAlive = m_System.GetParticles(m_Particles);
            if (numParticlesAlive < 1)
            {
                PosCache = new Vector4[m_System.main.maxParticles];
            }
            for (int i = 0; i < numParticlesAlive; i++)
            {
                if (IsDie)
                {
                    if (this.getStopAxis(m_Particles[i].position, this.fanwei, this.StopAxis))
                    {
                        m_Particles[i].remainingLifetime = 0;
                    }
                }
                else
                {
                    m_Particles[i].position = ClampVector3InBound(m_Particles[i].position, this.fanwei);
                    if(m_Particles[i].remainingLifetime > m_Particles[i].startLifetime * 0.9f){
                        this.PosCache[i].w = 0;
                    }
                    if (this.StopAxis != psExAxis.none)
                    {
                        if (this.PosCache[i].w > 0)
                        { 
                            m_Particles[i].position = this.PosCache[i];
                        }
                        
                        if (this.getStopAxis(m_Particles[i].position, this.fanwei, this.StopAxis))
                        {
                            this.PosCache[i] = new Vector4(m_Particles[i].position.x,m_Particles[i].position.y,m_Particles[i].position.z,1f);
                        }else{
                            this.PosCache[i] = new Vector4(m_Particles[i].position.x,m_Particles[i].position.y,m_Particles[i].position.z,0f);
                        }
                    }
                }
            }
            m_System.SetParticles(m_Particles, numParticlesAlive);
        }
        // Vector3 getWorldOrLocalPos(Vector3 pos, bool istoworld = true)
        // {
        //     if (istoworld)
        //         return this.isWorld ? this.transform.TransformPoint(pos) : pos;
        //     else
        //         return this.isWorld ? this.transform.InverseTransformPoint(pos) : pos;

        // }

        bool getStopAxis(Vector3 pos, Bounds rect, psExAxis mode)
        {
            // pos = getWorldOrLocalPos(pos);
            if (mode == psExAxis.none)
            {
                return false;
            }
            bool x = pos.x <= rect.min.x || pos.x >= rect.max.x;
            bool y = pos.y <= rect.min.y || pos.y >= rect.max.y;
            bool z = pos.z <= rect.min.z || pos.z >= rect.max.z;
            switch (mode)
            {
                case psExAxis.x:
                    return x;
                case psExAxis.y:
                    return y;
                case psExAxis.z:
                    return z;
                case psExAxis.xy:
                    return x || y;
                case psExAxis.yz:
                    return y || z;
                case psExAxis.xyz:
                    return x || y || z;
                default:
                    return false;
            }
        }

        Vector3 ClampVector3InBound(Vector3 pos, Bounds rect)
        {
            // Vector3 ppos = getWorldOrLocalPos(pos);
            pos.x = Mathf.Max(rect.min.x, Mathf.Min(rect.max.x, pos.x));
            pos.y = Mathf.Max(rect.min.y, Mathf.Min(rect.max.y, pos.y));
            pos.z = Mathf.Max(rect.min.z, Mathf.Min(rect.max.z, pos.z));
            // return getWorldOrLocalPos(pos, false);
            return pos;
        }

        void InitializeIfNeeded()
        {
            if (m_System == null)
                m_System = GetComponent<ParticleSystem>();

            if (m_Particles == null || m_Particles.Length < m_System.main.maxParticles)
            {
                m_Particles = new ParticleSystem.Particle[m_System.main.maxParticles];
                PosCache = new Vector4[m_System.main.maxParticles];
            }

        }
    }
}
