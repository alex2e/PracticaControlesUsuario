using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlesUsuario
{
    public class Contacto
    {
        #region ATRIBUTOS
        /// <summary>
        /// Atributos de tipo String: Nombre, Edad y DNI.
        /// </summary>
        private string nombre, dni;
        private int edad;
        #endregion

       
        public Contacto(string nombre, int edad, string dni)
        {
            this.nombre = nombre;
            this.edad = edad;
            this.dni = dni;
        }


        public string Nombre
        {
            get
            {
                return nombre;
            }
        }

        public String Edad
        {
            get
            {
                return edad.ToString();
            }
        }

        public string Dni
        {
            get
            {
                return dni;
            }
        }

        override
        public string ToString()
        {
            return string.Format("{0}     {1}     {2}", nombre, edad, dni);
        }

    }
}
