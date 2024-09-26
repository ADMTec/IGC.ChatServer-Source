using System;
using System.Collections.Generic;


namespace IniParser.Model
{
	public class SectionData : ICloneable
	{
		public SectionData(string sectionName) : this(sectionName, EqualityComparer<string>.Default)
		{
		}

		public SectionData(string sectionName, IEqualityComparer<string> searchComparer)
		{
			this._trailingComments = new List<string>();
			_searchComparer = searchComparer;
			if (string.IsNullOrEmpty(sectionName))
			{
				throw new ArgumentException("section name can not be empty");
			}
			this._leadingComments = new List<string>();
			this._keyDataCollection = new KeyDataCollection(this._searchComparer);
			this.SectionName = sectionName;
		}

		public SectionData(SectionData ori, IEqualityComparer<string> searchComparer = null)
		{
			this._trailingComments = new List<string>();
			this.SectionName = ori.SectionName;
			this._searchComparer = searchComparer;
			this._leadingComments = new List<string>(ori._leadingComments);
			this._keyDataCollection = new KeyDataCollection(ori._keyDataCollection, searchComparer ?? ori._searchComparer);
		}

		public void ClearComments()
		{
			this.LeadingComments.Clear();
			this.TrailingComments.Clear();
			this.Keys.ClearComments();
		}

		public void ClearKeyData()
		{
			this.Keys.RemoveAllKeys();
		}

		public void Merge(SectionData toMergeSection)
		{
			foreach (string item in toMergeSection.LeadingComments)
			{
				this.LeadingComments.Add(item);
			}
			this.Keys.Merge(toMergeSection.Keys);
			foreach (string item2 in toMergeSection.TrailingComments)
			{
				this.TrailingComments.Add(item2);
			}
		}

		public string SectionName
		{
			get
			{
				return this._sectionName;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this._sectionName = value;
				}
			}
		}

		[Obsolete("Do not use this property, use property Comments instead")]
		public List<string> LeadingComments
		{
			get
			{
				return this._leadingComments;
			}
			internal set
			{
				this._leadingComments = new List<string>(value);
			}
		}

		public List<string> Comments
		{
			get
			{
				return this._leadingComments;
			}
		}

		[Obsolete("Do not use this property, use property Comments instead")]
		public List<string> TrailingComments
		{
			get
			{
				return this._trailingComments;
			}
			internal set
			{
				this._trailingComments = new List<string>(value);
			}
		}

		public KeyDataCollection Keys
		{
			get
			{
				return this._keyDataCollection;
			}
			set
			{
				this._keyDataCollection = value;
			}
		}

		public object Clone()
		{
			return new SectionData(this, null);
		}

		private IEqualityComparer<string> _searchComparer;

		private List<string> _leadingComments;

		private List<string> _trailingComments;

		private KeyDataCollection _keyDataCollection;

		private string _sectionName;
	}
}
