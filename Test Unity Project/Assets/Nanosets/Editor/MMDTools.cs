using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Nanosoft
{

public class MMDTools
{
	
	/**
	 * Путь к каталогу в ассетах где лежат общие текстуры MMD
	 */
	public const string texPath = "Assets/MMD/Textures/";
	
	/**
	 * Открыть описание модели MMD
	 */
	private static MMDModel LoadSelectedModel()
	{
		// выделенный объект
		var obj = Selection.activeObject;
		if ( obj == null )
		{
			Debug.LogWarning("MMDTools: nothing selected");
			return null;
		}
		
		TextAsset asset = obj as TextAsset;
		if ( asset == null )
		{
			Debug.LogWarning("MMDTools: selection is not TextAsset (XML)");
			return null;
		}
		
		try
		{
			MMDModel model = MMDModel.Load(asset);
			if ( model == null )
			{
				Debug.LogWarning("MMDTools: fail to load model");
				return null;
			}
			
			model.assetPath = AssetDatabase.GetAssetPath(asset);
			return model;
		}
		catch (Exception e)
		{
			Debug.LogWarning("MMDTools: parse error\n" + e.ToString());
			return null;
		}
		
		//Debug.LogWarning("MMDTools: success!");
		//return null;
	}
	
	/**
	 * Проверить текстуры
	 */
	public static void FixTextures(MMDModel model, string prefix)
	{
		MMDTexture[] textures = model.textureList;
		int count = textures.Length;
		for(int i = 0; i < count; i++)
		{
			MMDTexture tex = textures[i];
			tex.texture = null;
			tex.assetPath = prefix + tex.fileName;
			TextureImporter ti = AssetImporter.GetAtPath(tex.assetPath) as TextureImporter;
			if ( ti == null )
			{
				tex.assetPath = texPath + tex.fileName;
				ti = AssetImporter.GetAtPath(tex.assetPath) as TextureImporter;
				if ( ti == null )
				{
					Debug.LogError("MMDTools: texture not found - " + tex.fileName);
					continue;
				}
				
				// текстуры в каталоге texPath не трогаем, пользователь должен
				// сам исправить ошибки, если они есть, только проверим и
				// вададим предупреждения
				
				if ( ti.textureShape != TextureImporterShape.Texture2D || ti.textureType != TextureImporterType.Default )
				{
					Debug.LogError("MMDTools: texture[" + tex.fileName + "] shape should be Texture2D and texture type should be Default");
				}
				
				continue;
			}
			
			bool dirty = false;
			
			// проверить TextureType=Default
			if ( ti.textureType != TextureImporterType.Default )
			{
				ti.textureType = TextureImporterType.Default;
				dirty = true;
			}
			
			// проверить TextureShape=Texture2D
			if ( ti.textureShape != TextureImporterShape.Texture2D )
			{
				ti.textureShape = TextureImporterShape.Texture2D;
				dirty = true;
			}
			
			// сохраним изменения, если они были
			if ( dirty )
			{
				ti.SaveAndReimport();
			}
			
			// загрузим текстуру
			tex.texture = AssetDatabase.LoadAssetAtPath(tex.assetPath, typeof(Texture2D)) as Texture2D;
			if ( tex.texture == null )
			{
				Debug.LogError("MMDTools: texture[" + tex.fileName + "] fail to load texture");
			}
		}
	}
	
	/**
	 * Проверить материалы
	 */
	public static bool FixMaterials(MMDModel model, string prefix)
	{
		// загрузим шейдер MMD Front
		Shader frontShader = Shader.Find("Toon/MMD Front");
		if ( frontShader == null )
		{
			Debug.LogError("MMDTools: shader not found [Toon/MMD Front]");
			return false;
		}
		
		// загрузим шейдер MMD Both
		Shader bothShader = Shader.Find("Toon/MMD Both");
		if ( bothShader == null )
		{
			Debug.LogError("MMDTools: shader not found [Toon/MMD Both]");
			return false;
		}
		
		MMDMaterial[] mats = model.materialList;
		int count = mats.Length;
		for(int i = 0; i < count; i++)
		{
			MMDMaterial mat = mats[i];
			
			Material m = AssetDatabase.LoadAssetAtPath(prefix + mat.name + ".mat", typeof(Material)) as Material;
			if ( m == null )
			{
				Debug.LogError("MMDTools: Mat[" + mat.name + "] not found, reimport .FBX first");
				continue;
			}
			
			m.shader = (mat.isDrawBothFaces == 0) ? frontShader : bothShader;
			m.renderQueue = -1;
			m.SetColor("_Color", Color.white);
			m.SetColor("_Diffuse", mat.diffuse.color4);
			m.SetColor("_Specular", mat.specular.color3);
			m.SetColor("_Ambient", mat.ambient.color3);
			//m.SetColor("_EdgeColor", mat.edgeColor.color4);
			//m.SetFloat("_Shiness", mat.shiness);
			
			if ( mat.mainTexId < 0 )
			{
				m.SetTexture("_MainTex", null);
			}
			else
			{
				MMDTexture mainTex = model.TextureById(mat.mainTexId);
				if ( mainTex == null )
				{
					Debug.LogError("MMDTools: Mat[" + mat.name + "] wrong main texture id");
				}
				else if ( mainTex.texture != null )
				{
					m.SetTexture("_MainTex", mainTex.texture);
				}
			}
			
			if ( mat.toonTexId < 0 )
			{
				m.SetTexture("_ToonTex", null);
			}
			else
			{
				MMDTexture toonTex = model.TextureById(mat.toonTexId);
				if ( toonTex == null )
				{
					Debug.LogError("MMDTools: Mat[" + mat.name + "] wrong toon texture id");
				}
				else if ( toonTex.texture != null )
				{
					m.SetTexture("_ToonTex", toonTex.texture);
				}
			}
			
			if ( mat.spTexId < 0 )
			{
				m.SetTexture("_SphereTex", null);
			}
			else
			{
				MMDTexture spTex = model.TextureById(mat.spTexId);
				if ( spTex == null )
				{
					Debug.LogError("MMDTools: Mat[" + mat.name + "] wrong sphere texture id");
				}
				else if ( spTex.texture != null )
				{
					m.SetTexture("_SphereTex", spTex.texture);
				}
			}
		}
		
		return true;
	}
	
	[MenuItem("Assets/Fix MMD materials")]
	public static void MenuFixMaterials()
	{
		MMDModel model = LoadSelectedModel();
		if ( model == null )
		{
			//Debug.LogWarning("MMDTools: fail to load model");
			return;
		}
		
		string prefix = Path.GetDirectoryName(model.assetPath) + "/";
		
		// загрузим и исправим текстуры
		FixTextures(model, prefix);
		
		// проверим и исправим материалы
		FixMaterials(model, prefix + "Materials/");
	}
	
	[MenuItem("Assets/Reset MMD materials")]
	protected static void MenuResetMaterials()
	{
		MMDModel model = LoadSelectedModel();
		if ( model == null )
		{
			//Debug.LogWarning("MMDTools: fail to load model");
			return;
		}
		
		string prefix = Path.GetDirectoryName(model.assetPath) + "/";
		
		MMDMaterial[] mats = model.materialList;
		int count = mats.Length;
		
		string matPrefix = prefix + "Materials/";
		
		for(int i = 0; i < count; i++)
		{
			MMDMaterial mat = mats[i];
			
			Material m = AssetDatabase.LoadAssetAtPath(matPrefix + mat.name + ".mat", typeof(Material)) as Material;
			
			if ( m == null )
			{
				Debug.LogError("MMDTools: Mat[" + mat.name + "] not found, reimport .FBX first");
				continue;
			}
			
			m.SetColor("_Color", Color.white);
			m.SetFloat("_LightFactor", 1f);
			m.SetFloat("_LightAtten", 1f);
			m.SetFloat("_DarkAtten", 0f);
		}
	}
}

[XmlRoot("MMDModel")]
public class MMDModel
{
	/**
	 * Пусть к ассету
	 */
	public string assetPath;
	
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
	
	[XmlIgnoreAttribute]
	public string assetPath;
	
	[XmlIgnoreAttribute]
	public Texture2D texture = null;
	
} // class MMDTexture

public class MMDMaterial
{
	
	[XmlElement("materialName")]
	public string name;
	
	[XmlElement("textureID")]
	public int mainTexId;
	
	[XmlElement("toonTextureID")]
	public int toonTexId;
	
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
	
} // class MMDMaterial

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
	
} // class MMDColor

} // namespace Nanosoft
