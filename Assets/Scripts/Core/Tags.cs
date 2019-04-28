using System;
using System.Collections.Generic;

[Serializable]
public class Tags<TagType>
{
	public List<TagType>				m_Tags = new List<TagType>();

	//////////////////////////////////////////////////////////////////////////
	public bool HasTag(TagType tag)
	{
		return m_Tags.Contains(tag);
	}
	public bool HasTag(params TagType[] tag)
	{
		if(tag != null)
			foreach (var n in tag)
				if(m_Tags.Contains(n) == false)		return false;

		return true;
	}
}

