using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfFinanciera.ServicioFinancieraIndependiente;

namespace WpfFinanciera.Utilidades
{
    public class UsuarioSingleton
    {
        private static Usuario _usuario;

        private UsuarioSingleton() { }

        public static Usuario ObtenerUsuario()
        {
            if (_usuario == null)
            {
                _usuario = new Usuario();
            }
            return _usuario;
        }

        public static void SettearUsuario(Usuario usuario)
        {
            _usuario = usuario;
        }
    }
}
