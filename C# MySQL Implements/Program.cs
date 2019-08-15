using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using System.Diagnostics;

namespace tryanddelete
{
    public class Program  {
          static void Main(string[] args) {

            String command = "clear";
            Process proc = new System.Diagnostics.Process ();
            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.Arguments = "-c \" " + command + " \"";
            proc.StartInfo.UseShellExecute = false; 
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start ();

            while (!proc.StandardOutput.EndOfStream) {
                Console.WriteLine (proc.StandardOutput.ReadLine ());
            }
                 
             MySqlConnection conn = null;
             MySqlDataReader rdr = null;
             MySqlCommand cmd;
             MySqlParameter param  = null;  
             MySqlDataAdapter da = null;
             MySqlTransaction tr = null;

             DataSet ds = new DataSet();

             string cs = @"server=localhost; database=mytestdb; uid=root; pwd=;";
             string query = null; 
             int x, z, no = 1, key;
             string y;

             AllMethods all = new AllMethods();

        try{
            conn = new MySqlConnection(cs);
            conn.Open();

            Console.WriteLine("MySQL version : {0}", conn.ServerVersion);
            Console.WriteLine("1-Select operation");
            Console.WriteLine("2-Insert Operation"); 
            Console.WriteLine("3-Delete Operation");
            Console.WriteLine("4-Update Operation");
            Console.WriteLine("5-Display in List");
            Console.WriteLine("6-Transaction Example");
            Console.WriteLine("7-Display In The Table");
            Console.WriteLine("8-Write Number Of Object");
            Console.WriteLine("9-Write In The DataTable");
            Console.WriteLine("10-Write Select Content In File(xml)"); 
            Console.WriteLine("Choose Your Operation");
            int choose = Convert.ToInt32(Console.ReadLine()); 
            Console.Clear(); 
            Console.WriteLine("Your Choose : "+choose);
            switch(choose){
            case 1 : 
                    all.addline();
                    query = "SELECT * FROM users";
                    cmd = new MySqlCommand(query, conn);
                    rdr = cmd.ExecuteReader();
                    Console.WriteLine("id"+" "+"name"+" "+"userkey");
                      while (rdr.Read()){
                        Console.WriteLine(rdr.GetInt32(0)+" - "+rdr.GetString(1)+" - "+rdr.GetInt32(2));
                                        }  break; 

            case 2 : 
                    Console.WriteLine("Enter id : "); x = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter name : "); y = Console.ReadLine();
                    Console.WriteLine("Enter userkey : "); z = Convert.ToInt32(Console.ReadLine());
                    cmd = new MySqlCommand(); cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO users(id, name, userkey) VALUES(@id, @name, @userkey)";
                    cmd.Prepare();  
                    cmd.Parameters.AddWithValue("@id", x);
                    cmd.Parameters.AddWithValue("@name", y);
                    cmd.Parameters.AddWithValue("@userkey", z);
                    cmd.ExecuteNonQuery();  break;

            case 3 : 
                     Console.WriteLine("Enter delete id : "); key = Convert.ToInt32(Console.ReadLine());
                     cmd = new MySqlCommand("DELETE FROM users WHERE id=@id", conn);
                     param = new MySqlParameter("@id", key);
                     cmd.Parameters.Add(param);
                     cmd.ExecuteNonQuery(); break;

            case 4 :
                    Console.WriteLine("Enter update id : "); key = Convert.ToInt32(Console.ReadLine()); 
                    Console.WriteLine("Enter new id : "); x = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter new name : "); y = Console.ReadLine();
                    Console.WriteLine("Enter new userkey : "); z = Convert.ToInt32(Console.ReadLine());
                    query = "update users set id ='"+x+"',name='"+y+"',userkey='"+z+"' where id='"+key+"';";  
                    cmd = new MySqlCommand(query, conn);   
                    rdr = cmd.ExecuteReader(); break;

            case 5 :   
                    cmd = new MySqlCommand(); cmd.Connection = conn;
                    cmd.CommandText = "SELECT * from users";
                    rdr = cmd.ExecuteReader();
                    StringBuilder sb = new StringBuilder(); 
                      while (rdr.Read()){
                       if (sb.Length > 0)
                           sb.Append(Environment.NewLine);

                        for (int i = 0; i < rdr.FieldCount; i++)
                           sb.AppendFormat("{0}    ", rdr[i]);                               
                    }
                    Console.WriteLine(sb.ToString()); break;   

            case 6 : 
                      try{
                        cmd = new MySqlCommand();
                        cmd.Connection = conn;
                        cmd.Transaction = tr;

                        cmd.CommandText = "UPDATE users SET namekey = 222 WHERE id = 1";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "UPDATE users SET id = 44 WHERE id = 3";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "UPDATE users SET name ='emiliano' WHERE id = 2";
                        cmd.ExecuteNonQuery();

                        tr.Commit();
                       }
                  catch (MySqlException ex){
                     try{ 
                          tr.Rollback();   
                        } 
                      catch (MySqlException ex1){
                             Console.WriteLine("Error: {0}",  ex1.ToString());                
                                                }
                            Console.WriteLine("Error: {0}",  ex.ToString());
                                  } break;

            case 7 : 
                    query = "SELECT * FROM users";
                     
                    cmd = new MySqlCommand(query, conn);
                    
                    rdr = cmd.ExecuteReader();
                    Console.WriteLine("-------------------------------------");
                    Console.WriteLine(" no| id  |    name     | userkey");
                    Console.WriteLine("-------------------------------------"); 
                        while ((rdr.Read())){
                          object a = rdr[0];
                          object b = rdr[1];
                          object c = rdr[2];
                          Console.WriteLine(String.Format("{0,-2} | {1,3} | {2,11} | {3,4}",no, a, b, c)); 
                          no++;
                    } break;  

            case 8 : 
                    query = "SELECT COUNT(*) FROM users";
                    cmd = new MySqlCommand(query, conn);
                    Console.WriteLine("Number Of Object : "+Convert.ToInt32(cmd.ExecuteScalar()));
                    break;   

            case 9 :  
                    query = "SELECT * FROM users";
                    da = new MySqlDataAdapter(query, conn); 
                    da.Fill(ds, "users"); 
                    DataTable dt = ds.Tables["Users"]; 
                    dt.WriteXml("users.xml");
                     foreach (DataRow row in dt.Rows)  {            
                        foreach (DataColumn col in dt.Columns)   {
                           Console.WriteLine(row[col]);
                                                                 }
                 Console.WriteLine("".PadLeft(20, '='));
                                                       }
                break;

            case 10 : 
                    query = "SELECT * FROM users";
                    da = new MySqlDataAdapter(query, conn); 
                    da.Fill(ds, "users"); 
                    dt = ds.Tables["Users"]; 
                    dt.WriteXml("users.xml"); 
                break; 

            case 404 : Console.WriteLine("bye"); break; 
            
            default : Console.WriteLine("Invalid Operation"); break;
          } 

        } catch (MySqlException ex){ 
                                    Console.WriteLine("An Error Detected : "+ex.ErrorCode+" "+ex.Message);
                                    Console.WriteLine("Error: {0}",  ex.ToString());

        } 
        finally{          
          if (conn != null){
              conn.Close();
          }
          if (rdr != null){
              rdr.Close();
          }
        }
         
         
        }    
    }
    public class AllMethods{
      public void addline(){
          int linelimit = 50; 
          for(int i = 1; i <= linelimit; i++){
                      Console.Write("-");
                      if(i == linelimit){
                                  Console.Write("\n");
                      }
                    }
        }
    }
}