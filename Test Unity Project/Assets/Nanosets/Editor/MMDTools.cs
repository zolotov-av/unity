using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;

public class MMDTools
{
	
	[MenuItem("Tools/Fix MMD")]
	public static void FixMMD()
	{
		var obj = Selection.activeObject;
		if ( obj == null )
		{
			Debug.Log("MMDTools.FixMMD(): nothing selected");
			return;
		}
		
		Debug.Log("MMDTools.FixMMD(): obj.name=" + obj.name);
		
		TextAsset ta = obj as TextAsset;
		if ( ta != null )
		{
			ProcessXML(ta);
			return;
		}
		
		Material mat = obj as Material;
		if ( mat != null )
		{
			Debug.Log("MMDTools.FixMMD(): selection is Material");
			Shader sh = mat.shader;
			Debug.Log("shader: " + sh.name);
			AssetDatabase.StartAssetEditing();
			mat.SetFloat("_LightAtten", 1.0f);
			AssetDatabase.StopAssetEditing();
			return;
		}
		
	}
	
	[MenuItem("Tools/Reset custom opts [MMD]")]
	public static void ResetCustomOptions()
	{
		var obj = Selection.activeObject;
		if ( obj == null )
		{
			return;
		}
		
		TextAsset ta = obj as TextAsset;
		if ( ta != null )
		{
			ProcessResetOpts(ta);
			return;
		}
	}
	
	protected static void ProcessXML(TextAsset ta)
	{
		Debug.Log("MMDTools.FixMMD(): selection is TextAsset");
		string prefix = Path.GetDirectoryName(AssetDatabase.GetAssetPath(ta)) + "/";
		Debug.Log("prefix: " + prefix);
		MMDModel model = MMDModel.Load(ta);
		if ( model == null )
		{
			Debug.Log("MMDTools.FixMMD(): fail to load model");
			return;
		}
		MMDMaterial[] mats = model.materialList;
		int count = mats.Length;
		Debug.Log("MMDTools.FixMMD(): model loaded, mat count: " + count);
		
		Shader shader = Shader.Find("Toon/Toon MMD");
		if ( shader == null )
		{
			Debug.Log("shader not found");
			return;
		}
		
		string matPrefix = prefix + "Materials/";
		for(int i = 0; i < count; i++)
		{
			MMDMaterial mat = mats[i];
			MMDTexture spTex = model.TextureById(mat.spTexId);
			string texName = spTex == null ? "none" : spTex.fileName;
			
			Material m = AssetDatabase.LoadAssetAtPath(matPrefix + mat.name + ".mat", typeof(Material)) as Material;
			string status = m != null ? "  ok  " : " fail ";
			
			if ( m == null )
			{
				Debug.Log("Mat[" + i.ToString() + "] " + mat.name + " ["  + status + "]");
				continue;
			}
			
			m.shader = shader;
			m.SetColor("_Color", Color.white);
			
			Texture2D t = AssetDatabase.LoadAssetAtPath(prefix + texName, typeof(Texture2D)) as Texture2D;
			status = t != null ? "  ok  " : " fail ";
			
			if ( t == null )
			{
				Debug.Log("Mat[" + i.ToString() + "] " + mat.name + " spTex = " + texName + " [" + status + "]");
			}
			else
			{
				m.SetTexture("_SphereAddTex", t);
			}
		}
	}
	
	protected static void ProcessResetOpts(TextAsset ta)
	{
		string prefix = Path.GetDirectoryName(AssetDatabase.GetAssetPath(ta)) + "/";
		MMDModel model = MMDModel.Load(ta);
		if ( model == null )
		{
			Debug.Log("MMDTools.ProcessResetOpts(): fail to load model");
			return;
		}
		
		MMDMaterial[] mats = model.materialList;
		int count = mats.Length;
		Debug.Log("MMDTools.FixMMD(): model loaded, mat count: " + count);
		
		string matPrefix = prefix + "Materials/";
		
		for(int i = 0; i < count; i++)
		{
			MMDMaterial mat = mats[i];
			
			Material m = AssetDatabase.LoadAssetAtPath(matPrefix + mat.name + ".mat", typeof(Material)) as Material;
			string status = m != null ? "  ok  " : " fail ";
			
			if ( m == null )
			{
				Debug.Log("Mat[" + i.ToString() + "] " + mat.name + " ["  + status + "]");
				continue;
			}
			
			m.SetFloat("_LightAtten", 1f);
			m.SetFloat("_DarkAtten", 0f);
			m.SetFloat("_LightFactor", 1f);
			m.SetColor("_Color", Color.white);
		}
	}
}

[XmlRoot("MMDModel")]
public class MMDModel
{
	
	[XmlArray("textureList")]
    [XmlArrayItem("Texture")]
	public MMDTexture[] textureList;
	
	[XmlArray("materialList")]
    [XmlArrayItem("Material")]
	public MMDMaterial[] materialList;
	
	public MMDTexture TextureById(int id)
	{
		if ( id < 0 ) return null;
		if ( id >= textureList.Length ) return null;
		return textureList[id];
	}
	
	public static MMDModel Load(TextAsset asset)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(MMDModel));
		StringReader reader = new StringReader(asset.text);
		MMDModel model = serializer.Deserialize(reader) as MMDModel;
		return model;
	}
	
} // class MMDModel

public class MMDTexture
{
	[XmlElement("fileName")]
	public string fileName;
}

public class MMDMaterial
{
	[XmlElement("materialName")]
	public string name;
	
	[XmlElement("additionalTextureID")]
	public int spTexId;
}
