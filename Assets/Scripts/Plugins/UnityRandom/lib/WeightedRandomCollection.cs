﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace URandom
{
	public class WeightedRandomCollection<T> : IEnumerable<T>
	{
		private	UnityRandom				m_Generator;
		private float					m_Range;
		private Dictionary<T, float>	m_Data = new Dictionary<T, float>(10);
		public Dictionary<T, float>		Data{ get{ return m_Data; } }
		
		public bool		IsEmpty{ get{ return m_Data.Count == 0; } }
		public float	FullWeight
		{
			get
			{
				return m_Range; 
			}

			set
			{
				var scale = m_Range / value;
				m_Range = value;

				foreach(var n in m_Data.Keys)
					m_Data[n] *= scale;
			}  
		}

		//////////////////////////////////////////////////////////////////////////
		public void Apply(T value, float weight)
		{
			float valueWeight;
			if(m_Data.TryGetValue(value, out valueWeight))
			{	// set add weight to walue
				var newWeight = valueWeight + weight;
				if(newWeight <= 0.0f)
				{
					m_Data.Remove(value);
					m_Range -= valueWeight;
					return;
				}
				m_Data[value] = valueWeight + weight;
			}
			else
				if(weight > 0.0f)	m_Data[value] = weight;
				else				return;

			m_Range += weight;
		}

		public T Next()
		{
			var pos = m_Generator.Range(0.0f, m_Range);
			foreach(var n in m_Data)
			{
				if(pos <= n.Value)
					return n.Key;
				pos -= n.Value;
			}

			throw new IndexOutOfRangeException();
		}

		public T Next(float selectionCost)
		{
			var pos = m_Generator.Range(0.0f, m_Range);
			foreach(var n in m_Data)
			{
				if(pos <= n.Value)
				{
					var result = n.Key;
					var newWeight = n.Value - selectionCost;

					if(newWeight <= 0.0f)			m_Data.Remove(n.Key);
					else							m_Data[n.Key] = newWeight;

					return n.Key;
				}
				pos -= n.Value;
			}

			throw new IndexOutOfRangeException();
		}

		public T Next(float selectionCost, ref float constExcess)
		{
			var pos = m_Generator.Range(0.0f, m_Range);
			foreach(var n in m_Data)
			{
				if(pos <= n.Value)
				{
					var result = n.Key;
					var newWeight = n.Value - selectionCost;

					if(newWeight <= 0.0f)			{ m_Data.Remove(n.Key); constExcess = -newWeight; }
					else							m_Data[n.Key] = newWeight;

					return n.Key;
				}
				pos -= n.Value;
			}

			throw new IndexOutOfRangeException();
		}

		public WeightedRandomCollection(UnityRandom randomGenerator)
		{
			m_Generator = randomGenerator;
		}

		#region IEnumerable<T> Members

		/// <summary>
		/// </summary>
		/// <returns>A sequence of random elements from the bag.</returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			foreach(var n in m_Data)
				yield return n.Key;
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// </summary>
		/// <returns>A sequence of random elements from the bag.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		#endregion
	}

	public class VotedRandomCollection<T> : IEnumerable<T>
	{
		private	UnityRandom				m_Generator;
		private Dictionary<T, float>	m_Data = new Dictionary<T, float>(10);

		public bool		IsEmpty{ get{ return m_Data.Count == 0; } }

		//////////////////////////////////////////////////////////////////////////
		public void Apply(T value, float weight)
		{
			float valueWeight;
			if(m_Data.TryGetValue(value, out valueWeight))
			{	// set add weight to walue
				var newWeight = valueWeight + weight;
				if(newWeight <= 0.0f)
				{
					m_Data.Remove(value);
					return;
				}
				m_Data[value] = valueWeight + weight;
			}
			else
				if(weight > 0.0f)	m_Data[value] = weight;
				else				return;
		}

		public T Next()
		{
			var winner = default(T);
			float winnerVote = 0.0f;

			foreach(var n in m_Data)
			{
				var vote = m_Generator.Range(0.0f, n.Value);
				if(winnerVote <= vote)
				{
					winner = n.Key;
					winnerVote = vote;
				}
			}

			return winner;
		}

		public VotedRandomCollection(UnityRandom randomGenerator)
		{
			m_Generator = randomGenerator;
		}

		#region IEnumerable<T> Members

		/// <summary>
		/// </summary>
		/// <returns>A sequence of random elements from the bag.</returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			foreach(var n in m_Data)
				yield return n.Key;
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// </summary>
		/// <returns>A sequence of random elements from the bag.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		#endregion
	}
	
	public class VotedRandomMinMax<T> : IEnumerable<T>
	{
		private	UnityRandom					m_Generator;
		private Dictionary<T, Vector2>		m_Data = new Dictionary<T, Vector2>(10);

		public bool		IsEmpty{ get{ return m_Data.Count == 0; } }

		//////////////////////////////////////////////////////////////////////////
		public void Apply(T value, float min, float max)
		{
			Vector2 weight = new Vector2(min, max);
			Vector2 valueWeight;
			if(m_Data.TryGetValue(value, out valueWeight))
			{	// set add weight to walue
				var newWeight = valueWeight + weight;
				if(newWeight.x >= newWeight.y)
				{
					m_Data.Remove(value);
					return;
				}
				m_Data[value] = valueWeight + weight;
			}
			else
				if(weight.x >= weight.y)	m_Data[value] = weight;
				else						return;
		}

		public T Next()
		{
			var winner = default(T);
			float winnerVote = 0.0f;

			foreach(var n in m_Data)
			{
				var vote = m_Generator.Range(n.Value.x, n.Value.y);
				if(winnerVote <= vote)
				{
					winner = n.Key;
					winnerVote = vote;
				}
			}

			return winner;
		}

		public VotedRandomMinMax(UnityRandom randomGenerator)
		{
			m_Generator = randomGenerator;
		}

		#region IEnumerable<T> Members

		/// <summary>
		/// </summary>
		/// <returns>A sequence of random elements from the bag.</returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			foreach(var n in m_Data)
				yield return n.Key;
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// </summary>
		/// <returns>A sequence of random elements from the bag.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)this).GetEnumerator();
		}

		#endregion
	}

	public class Pseudo
	{
		public float		m_Min;
		public float		m_Max;

		protected float		m_Magnitude;
		protected float		m_HalfMagnitude;

		public float		m_MinTweek;
		public float		m_MaxTweek;

		public int				m_Distance;
		public Queue<float>		m_Calls;
		public float			m_AverageWeight;

		//////////////////////////////////////////////////////////////////////////
		public float Next()
		{
			float result = UnityEngine.Random.Range(m_Min - m_MinTweek, m_Max - m_MaxTweek);
			float weight = result - m_Min;
			m_Calls.Enqueue(weight);
			m_AverageWeight -= m_Calls.Dequeue() / m_Distance;
			m_AverageWeight += result / m_Distance;
			
			var dif = m_AverageWeight - m_HalfMagnitude;
			if(dif >= 0)
			{
				
				m_MaxTweek = (dif / m_HalfMagnitude) * m_Magnitude;
				m_MinTweek = 0.0f;
			}
			else
			{
				m_MinTweek = (dif / m_HalfMagnitude) * m_Magnitude;
				m_MaxTweek = 0.0f;
			}

			return result;
		}

		//
		public Pseudo(float min, float max, int discance)
		{
			m_Min = min;
			m_Max = max;
			m_Magnitude = m_Max - m_Min;
			m_HalfMagnitude = m_Magnitude * 0.5f;

			m_Distance = discance;

			m_AverageWeight = m_HalfMagnitude;
			m_Calls = new Queue<float>(m_Distance);
			for(var n = 0; n < m_Distance; ++ n)
				m_Calls.Enqueue(m_HalfMagnitude);
		}
	}
}