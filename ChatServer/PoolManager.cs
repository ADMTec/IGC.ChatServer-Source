using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatServer
{
	internal class PoolManager
	{
		public PoolManager(int count)
		{
			this._count = count;
			this.initPool();
		}

		private void initPool()
		{
			for (int i = this._count - this._pool.Count; i > 0; i--)
			{
				this._pool.Add((this._pool.Count == 0) ? 0 : (this._pool.Max() + 1));
			}
		}

		public int Dequeue()
		{
			object accessLocker = this._accessLocker;
			int result;
			lock (accessLocker)
			{
				int num = this._pool.First<int>();
				this._pool.Remove(num);
				this._destributed.Add(num);
				this.initPool();
				result = num;
			}
			return result;
		}

		public void Enqueue(int i)
		{
			object accessLocker = this._accessLocker;
			lock (accessLocker)
			{
				this._destributed.Remove(i);
				this._pool.Insert(i, i);
			}
		}

		private object _accessLocker = new object();

		private List<int> _pool = new List<int>();

		private List<int> _destributed = new List<int>();

		private int _count;
	}
}
