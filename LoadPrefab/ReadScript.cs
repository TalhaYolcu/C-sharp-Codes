
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Threading;

public class ReadScript : MonoBehaviour
{

    public GameObject prefab;
    public Texture myTexture=null;
    public Material myMaterial=null;

    private string path1;
    private string create_target1; 

        public static List<string> texture_paths=new List<string>();
        public static List<string> mesh_paths=new List<string>();
        public static List<string> material_paths=new List<string>();

        public bool isTexture(string path) {
            if (path.Contains(".jpg") || path.Contains(".png")) {
                return true;
            }
            return false;
        }
        public bool isMesh(string path) {
            if(path.Contains(".obj") || path.Contains(".fbx") || path.Contains(".dxf") || path.Contains(".dae")) {
                return true;
            }
            return false;
        }
        public bool isMaterial(string path) {
            if(path.Contains(".mat")) {
                return true;
            }
            return false;            
        }

        public void read_dir(string path) {
            string[] allfiles=Directory.GetFileSystemEntries(path,"*.*",SearchOption.AllDirectories);
            foreach(var file in allfiles) {
                FileInfo info=new FileInfo(file);
                System.Console.WriteLine(info.FullName);
                if(!info.FullName.Contains(".")){
                    read_dir(info.FullName);
                }
                if(isTexture(info.FullName)) {
                    texture_paths.Add(info.FullName);
                }
                else if(isMesh(info.FullName)) {
                    mesh_paths.Add(info.FullName);
                }
                else if(isMaterial(info.FullName)) {
                    material_paths.Add(info.FullName);
                }
                
                //Console.WriteLine(file);
            }  
        }
        public void create_required_directories(string path) {
            if(!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }  
            if(!Directory.Exists(path+"\\Textures")) {
                Directory.CreateDirectory(path+"\\Textures");      
            }
            if(!Directory.Exists(path+"\\Materials")) {
                Directory.CreateDirectory(path+"\\Materials");      
            }  
            if(!Directory.Exists(path+"\\Meshes")) {
                Directory.CreateDirectory(path+"\\Meshes");      
            }                              
   

        }
        public int get_last_slash(string str) {
            return str.LastIndexOf('\\');
        }
        public void copy_files(string path) {
            foreach(string x in texture_paths) {
                int index=get_last_slash(x);
                string file_name=x.Substring(index);
                if(!File.Exists(path+"\\Textures"+file_name)) {
                    File.Copy(x,path+"\\Textures"+file_name);
                }
            }
            foreach(var x in mesh_paths) {
                int index=get_last_slash(x);
                string file_name=x.Substring(index);  
                if(!File.Exists(path+"\\Meshes"+file_name)) {
                    File.Copy(x,path+"\\Meshes"+file_name);   
                }                              
            }
            foreach(var x in material_paths) {
                int index=get_last_slash(x);
                string file_name=x.Substring(index);   
                if(!File.Exists(path+"\\Materials"+file_name)) {
                    File.Copy(x,path+"\\Materials"+file_name); 
                }                             
            }                        
        }  
        private void fill_list(List<string> mylist,char x) {
            switch (x)
            {
                case '1':
                    for(int i=0;i<mesh_paths.Count;i++) {
                        int index=get_last_slash(mesh_paths[i]);
                        mylist.Add(mesh_paths[i].Substring(index+1));
                    }
                    break;
                case '2':
                    for(int i=0;i<texture_paths.Count;i++) {
                        int index=get_last_slash(texture_paths[i]);
                        mylist.Add(texture_paths[i].Substring(index+1));
                    }
                    break;                    
                case '3':
                    for(int i=0;i<material_paths.Count;i++) {
                        int index=get_last_slash(material_paths[i]);
                        mylist.Add(material_paths[i].Substring(index+1));
                    }
                    break;
                default:
                    break;
            }
            
        }  

    private string erase_file_extension(string path) {
        int index=0;
        for(int i=0;i<path.Length;i++) {
            if(path[i]=='.') {
                index=i;
            }
        }
        return path.Substring(0,index);
    }
    public void OpenFileBrowser() {
        create_target1=@"D:\GTU\UnityProjects\ReadFile\Assets\Resources\LoadedObject";
        path1=EditorUtility.OpenFolderPanel("Select source folder","","");
        string asset_path="Assets/Resources/LoadedObject/Meshes/";
        string texture_path="LoadedObject/Textures/";
        string material_path="LoadedObject/Materials/";
        /*
        1-Obje oluştur
        2-Obje verdiğin obj dosyasını kullansın
        3-Material oluştur, textureları material a at
        4-Objeye material ata
        */
        read_dir(path1);
        create_required_directories(create_target1);
        copy_files(create_target1);
        AssetDatabase.Refresh();

        List<string> loaded_meshes=new List<string>();
        List<string> loaded_textures=new List<string>();
        List<string> loaded_materials=new List<string>();

        fill_list(loaded_meshes,'1');
        fill_list(loaded_textures,'2');
        fill_list(loaded_materials,'3');


        asset_path=asset_path+loaded_meshes[0];
        texture_path=texture_path+loaded_textures[0];
        material_path=material_path+loaded_materials[0];

        texture_path=erase_file_extension(texture_path);
        material_path=erase_file_extension(material_path);

       

        //create game object
        //GameObject clone=(GameObject)Instantiate(Resources.Load("Sideboard")); 
        var modelRoot=(GameObject)AssetDatabase.LoadMainAssetAtPath(asset_path);

        GameObject clone=(GameObject)PrefabUtility.InstantiatePrefab(modelRoot);
        if(clone==null) {
            print("NULL 1");
        }

        //prefab varsa alabilirsin 
        //.LOD fonksiyonu farklı yakınlık için farklı resolution

        //get texture
        print(asset_path);
        print(texture_path);
        print(material_path);
        
        myTexture=(Texture)Resources.Load<Texture>(texture_path);

        //get material
        myMaterial=(Material)Resources.Load<Material>(material_path);


    
        //give texture to the material
        myMaterial.mainTexture = myTexture;


        clone.AddComponent<MeshRenderer>();
        clone.GetComponent<MeshRenderer>().material=myMaterial;

        //apply material to the all childs
        Renderer[] children;
        children = clone.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in children){
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++){
                mats[j] = myMaterial;
            }
            rend.materials = mats;
        }
        clone.transform.position=new Vector3(0,0,0);

    }

    // Start is called before the first frame update
    void Start(){   

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
