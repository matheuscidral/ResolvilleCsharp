using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;

namespace Repositories
{
    public class UsuarioRepository {
        private static MySqlConnection conexao; // CONEXÃO com o BANCO DE DADOS
        static List<Models.Usuario> usuarios = new List<Models.Usuario>(); // LISTA ou CACHE do SOFTWARE.
        
        // BANCO DE DADOS //
        public static void InitConexao(){ // INICIAR conexão com o BANCO DE DADOS.
            string info = "server=localhost;database=resolville;user id=root;password=''";
            conexao = new MySqlConnection(info);
            try{
                conexao.Open();
            }
            catch{
                MessageBox.Show("Impossível estabelecer conexão com o banco");
            }
        }
         public static void CloseConexao(){ // FINALIZAR conexão com o BANCO DE DADOS.
            conexao.Close();
        }
        public static List<Models.Usuario> Sincronizar(){ // SINCRONIZAR o banco de dados atual com o SOFTWARE.
            InitConexao();
            string query = "SELECT * FROM usuario"; // talvez mude
            MySqlCommand command = new MySqlCommand(query, conexao);
            MySqlDataAdapter bdAdapter = new MySqlDataAdapter(command);

            DataSet dbDataSet = new DataSet();
            bdAdapter.Fill(dbDataSet, "usuario");
            DataTable table = dbDataSet.Tables["usuario"];

            foreach(DataRow row in table.Rows){
                int id = Convert.ToInt32(row["ID_Usuario"].ToString());
                Models.Usuario usuario = new Models.Usuario(); // possivelmente tem q tirar o construct
                usuario.Id = id;
                usuario.Nome = row["Nome"].ToString();
                usuario.Apelido = row["Nome_Usuario"].ToString();
                usuario.Email = row["Email"].ToString();
                usuario.Cpf = row["CPF"].ToString();
                usuario.Senha = row["Senha"].ToString();
                usuarios.Add(usuario);
            }
            CloseConexao();
            return usuarios;
        }
        
        // INFORMAÇÕES DO USUÁRIO //
        public static Models.Usuario? GetUsuario(int index){ // PUXAR informações do usuário a partir do INDEX recebido.
            if(index < 0 || index >= usuarios.Count){
                return null;
            }else {
                return usuarios[index];
            }
        }
        public static void AddUsuario(Models.Usuario usuario){ // ADICIONAR as informações do usuário no BANCO DE DADOS e na LISTA (cache).
            InitConexao();
            string query = "INSERT INTO usuario (Nome, CPF, Email, Nome_Usuario, Senha) VALUES (@Nome, @CPF, @Email, @Nome_Usuario, @Senha)";
            MySqlCommand command = new MySqlCommand(query, conexao);
            if(usuario != null){
                command.Parameters.AddWithValue("@Nome", usuario.Nome);
                command.Parameters.AddWithValue("@CPF", usuario.Cpf);
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@Nome_Usuario", usuario.Apelido);
                command.Parameters.AddWithValue("@Senha", usuario.Senha);

                int rowsAffected = command.ExecuteNonQuery();
                usuario.Id = Convert.ToInt32(command.LastInsertedId);

                if(rowsAffected > 0){
                    usuarios.Add(usuario);
                }else{
                    MessageBox.Show("Usuário não Cadastrado!");
                }
            }else{
                MessageBox.Show("Usuário não Cadastrado!");
            }
            CloseConexao();
        }
        public static List<Models.Usuario> ListarUsuarios() { // LISTAR as informações dos usuários do BANCO DE DADOS.
            return usuarios;
        }
        public static void UpdateUsuario(int index, Models.Usuario usuario){ // ATUALIZAR as informações dos usuários no BANCO DE DADOS e na LISTA (cache) a partir do INDEX recebido.
            InitConexao();
            string query = "UPDATE usuario SET Nome = @Nome, CPF = @CPF, Email = @Email, Nome_Usuario = @Nome_Usuario, Senha = @Senha WHERE id = @Id";
            MySqlCommand command = new MySqlCommand(query, conexao);
            if(usuario != null){
                command.Parameters.AddWithValue("@Nome", usuario.Nome);
                command.Parameters.AddWithValue("@CPF", usuario.Cpf);
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@Nome_Usuario", usuario.Apelido);
                command.Parameters.AddWithValue("@Senha", usuario.Senha);
                command.Parameters.AddWithValue("@ID_Usuario", usuario.Id);
                int rowsAffected = command.ExecuteNonQuery();
            
                if (rowsAffected > 0) {
                    usuarios[index] = usuario;
                }
                else {
                    MessageBox.Show(rowsAffected.ToString());
                }
            }else {
                MessageBox.Show("Usuário não encontrado");
            }
            CloseConexao();
        }
        public static void DeleteUsuario(int index){ // DELETAR as informações do usuário no BANCO DE DADOS e na LISTA (cache) a partir do INDEX recebido.
            InitConexao();
            string query = "DELETE FROM usuario WHERE ID_Pessoa = @ID_Pessoa";
            MySqlCommand command = new MySqlCommand(query, conexao);
            int rowsAffected = command.ExecuteNonQuery();

            if(rowsAffected > 0){
                usuarios.RemoveAt(index);
            }else{
                MessageBox.Show("Usuário não excluído.");
            }

            CloseConexao();
        }
    }
}