using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;

namespace Nanosoft
{

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
		
		Shader frontShader = Shader.Find("Toon/MMD Front");
		if ( frontShader == null )
		{
			Debug.LogError("shader not found [Toon/MMD Front]");
			return;
		}
		
		Shader bothShader = Shader.Find("Toon/MMD Both");
		if ( bothShader == null )
		{
			Debug.LogError("shader not found [Toon/MMD Both]");
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
			
			m.shader = (mat.isDrawBothFaces != 0) ? bothShader : frontShader;
			m.renderQueue = -1;
			m.SetColor("_Color", Color.white);
			m.SetColor("_Diffuse", mat.diffuse.color4);
			m.SetColor("_Specular", mat.specular.color3);
			m.SetColor("_Ambient", mat.ambient.color3);
			//m.SetColor("_EdgeColor", mat.edgeColor.color4);
			//m.SetFloat("_Shiness", mat.shiness);
			
			Texture2D t = AssetDatabase.LoadAssetAtPath(prefix + texName, typeof(Texture2D)) as Texture2D;
			status = t != null ? "  ok  " : " fail ";
			
			if ( t == null )
			{
				Debug.Log("Mat[" + i.ToString() + "] " + mat.name + " spTex = " + texName + " [" + status + "]");
			}
			else
			{
				m.SetTexture("_SphereTex", t);
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
	
	[XmlElement("diffuse")]
	public MMDColor diffuse;
	
	[XmlElement("specular")]
	public MMDColor specular;
	
	[XmlElement("ambient")]
	public MMDColor ambient;
	
	[XmlElement("edgeColor")]
	public MMDColor edgeColor;
	
	[XmlElement("shiness")]
	public float shiness;
	
	[XmlElement("isDrawBothFaces")]
	public int isDrawBothFaces;
}

public class MMDColor
{
	[XmlElement("r")]
	public float r = 0.0f;
	
	[XmlElement("g")]
	public float g = 0.0f;
	
	[XmlElement("b")]
	public float b = 0.0f;
	
	[XmlElement("a")]
	public float a = 1.0f;
	
	public Color color3
	{
		get { return new Color(r, g, b, 1.0f); }
	}
	
	public Color color4
	{
		get { return new Color(r, g, b, a); }
	}
}

} // namespace Nanosoft
