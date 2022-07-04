using System;
using System.Globalization;

namespace ReadObjectFile {

    class Shape {
        private int vertice_index{get; set; }
        private int normal_index{get; set; }
        private int texture_index{get; set; }

/*        public int get_vertice_index() {
            return vertice_index;
        }
        public int get_normal_index() {
            return normal_index;
        }
        public int get_texture_index() {
            return texture_index;
        }
        */
    }

    class Point {
        private double x {get; set; }
        private double y{get; set; }
        private double z{get; set; }

        private int index{get; set; }

        public Point(double xx,double yy,double zz,int i) {
            x=xx;
            y=yy;
            z=zz;
            index=i;
        }
        /*public double get_x() {
            return x;
        }
        public double get_y() {
            return y;
        }
        public double get_z() {
            return z;
        }
        public int get_index() {
            return index;
        }
        public void set_x(double v) {
            x=v;
        }
        public void set_y(double v) {
            y=v;
        }
        public void set_z(double v) {
            z=v;
        }
        public void set_index(int v) {
            index=v;
        }*/
        public override string ToString()
        {
            return index+ ") " + x + " " + y + " " + " " + z + " "; 
        }
    }

    class Read {
        public static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("../MyObject/Meshes/Chair.obj");  
            List<string> vertices=new List<string>();
            List<string> vertex_normals = new List<string>();
            List<string> textures=new List<string>();
            List<string> polygons=new List<string>();

            string my_offset="a";

            
            for (int i=0;i<lines.Length-1;i++) {
                if(lines[i].Length!=0) {
                    if(lines[i][0]=='v' && lines[i][1]==' ') {
                        vertices.Add(lines[i]);
                    }
                    else if(lines[i][0]=='v' && lines[i][1]=='n') {
                        vertex_normals.Add(lines[i]);
                    }
                    else if(lines[i][0]=='v' && lines[i][1]=='t') {
                        textures.Add(lines[i]);
                    }
                    else if(lines[i][0]=='s') {
                        my_offset=lines[i].Substring(2);
                    }
                    else if(lines[i][0]=='f') {
                        polygons.Add(lines[i]);
                    }
                }
            }

            Console.WriteLine("Vertice length:{0:D}",vertices.Count);
            Console.WriteLine("Vertex normal length:{0:D}",vertex_normals.Count);
            Console.WriteLine("Texture length:{0:D}",textures.Count);
            Console.WriteLine("Polygon length:{0:D}",polygons.Count);

            List<Shape> my_shapes=new List<Shape>();

            parse_shapes(my_shapes,vertices,vertex_normals,textures,polygons);




        }

        public static void parse_shapes(List<Shape> my_shapes,List<string>vertices,List<string>vertex_normals,List<string>textures,List<string>polygons) {
            List<Point> vertice_points=new List<Point>();
            List<Point> normal_points=new List<Point>();
            List<Point> texture_points=new List<Point>();

            parse_points(vertices,vertice_points,"v");
            parse_points(vertex_normals,normal_points,"vn");
            parse_points(textures,texture_points,"vt");

            foreach(var Point in vertice_points) {
                Console.WriteLine(Point);
            }
            Console.WriteLine("---");
            foreach(var Point in normal_points) {
                Console.WriteLine(Point);
            }
            Console.WriteLine("---");

            foreach(var Point in texture_points) {
                Console.WriteLine(Point);
            }                        

        }

        public static void parse_points(List<string> vertices,List<Point> points,string selector) {
            for(int i=0;i<vertices.Count;i++) {
                string[] words=vertices[i].Split(' ');
                List<string> parsed_ones=new List<string>();
                for(int k=0;k<words.Length;k++) {
                    if(words[k].Length!=0) {
                        parsed_ones.Add(words[k]);
                    }
                }
                if(String.Compare(parsed_ones[0],selector)==0) {
                    
                    try
                    {
                        NumberFormatInfo provider = new NumberFormatInfo();
                        provider.NumberDecimalSeparator = "."; 
                        provider.NumberGroupSeparator = ",";                           

                        double x=Convert.ToDouble(parsed_ones[1],provider);
                        double y=Convert.ToDouble(parsed_ones[2],provider);
                        double z=Convert.ToDouble(parsed_ones[3],provider);


                        Point p=new Point(x,y,z,i);
                        points.Add(p);
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }



                }                    
            }
        }
    }
}