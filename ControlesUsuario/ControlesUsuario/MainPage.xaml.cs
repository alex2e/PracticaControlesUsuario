using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ControlesUsuario
{
    public partial class MainPage : ContentPage
    {
        #region Atributos
        /// <summary>
        /// Lista de contactos
        /// </summary>
        ObservableCollection<Contacto> contactos = new ObservableCollection<Contacto>();
        ObservableCollection<Contacto> contactosMostrar = new ObservableCollection<Contacto>();
        MatchCollection matches;
        //ObservableCollection<Contacto> listaVacia = new ObservableCollection<Contacto>();
        #endregion

        #region Propiedades

        #endregion

        #region Eventos
            

        #endregion

        #region Métodos
        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            btnExaminar.Clicked += (sender, args) =>
            {
                if(pickerSeleccion.SelectedIndex == 0)
                {
                    LeerContactos();
                    lvwListaUsuarios.ItemsSource = contactos;
                }

            };

            btnBuscar.Clicked += (sender, args) =>
            {
                buscar();
            };
        }

        /// <summary>
        /// Este metodo inserta en una lista de contactos los que saca del archivo info.txt.txt
        /// </summary>
        public void LeerContactos()
        {
            int counter = 0;
            string line;
            Contacto contacto;
            
            string nombre = "";
            int edad = 0;
            string nif = "";
            string ruta = "ControlesUsuario.Assets.Info.txt.txt";
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream(ruta);


            //Leer el fichero y mostrarlo línea a línea.
            System.IO.StreamReader fichero = new System.IO.StreamReader(stream);

            /*Bucle encargado de leer el fichero, ir almacenando los valores correspondientes
             a nombre, edad y nif e ir almacenándolos en un nuevo contacto.*/
            while ((line = fichero.ReadLine()) != null)
            {
                
                counter++;

                switch (counter)
                {
                    case 1:
                        nombre = line;
                        break;
                    case 2:
                        int.TryParse(line, out edad);
                        break;
                    case 3:
                        nif = line;
                        contacto = new Contacto(nombre, edad, nif);
                        contactos.Add(contacto);
                        counter = 0;
                        break;
                }
            }
        }

        /// <summary>
        ///  Muestra al usuario información sobre un error que él esta cometiendo
        /// </summary>
        /// <param name="mensaje"></param>
        private void lanzarAdvertencia(String mensaje)
        {
            DisplayAlert(mensaje, "ERROR", mensaje, "ACEPTAR");
        }

        /// <summary>
        /// Busca en el array que ha generado el leerArchivo con los valores indicados, sino se indica un error.
        /// </summary>
        private void buscar()
        {
            if(EntryBuscarPorNombre.Text == null)
            {
                EntryBuscarPorNombre.Text = "";
            }
            if(EntryEdadMaxima.Text == null)
            {
                EntryEdadMaxima.Text = "";
            }
            if(EntryEdadMinima.Text == null)
            {
                EntryEdadMinima.Text = "";
            }

            int edad1;

            //Si no hay campos vacios
            if (!EntryEdadMaxima.Text.Equals("") && !EntryEdadMinima.Text.Equals("") && !EntryBuscarPorNombre.Text.Equals(""))
            {
                //Se hace control numerico
                controlNumerico();
            }

            else if (!EntryEdadMaxima.Text.Equals("") && !EntryEdadMinima.Text.Equals(""))
            {
                //Se hace control numerico
                controlNumerico();
            }
            else if (!EntryEdadMaxima.Text.Equals(""))
            {
                if (int.TryParse(EntryEdadMaxima.Text.Trim(), out edad1))
                {
                    //Se hace control numerico
                    realizarBusqueda();
                }
                else
                {
                    EntryEdadMaxima.Text = EntryEdadMinima.Text = "";
                    lanzarAdvertencia("Ningun campo edad puede componerse por letras, solo aceptan valores numéricos.");
                }
            }
            else if (!EntryEdadMinima.Text.Equals(""))
            {
                if (int.TryParse(EntryEdadMinima.Text.Trim(), out edad1))
                {
                    //Se hace control numerico
                    realizarBusqueda();
                }
                else
                {
                    EntryEdadMaxima.Text = EntryEdadMinima.Text = "";
                    lanzarAdvertencia("Ningun campo edad puede componerse por letras, solo aceptan valores numéricos.");
                }
            }
            else { realizarBusqueda(); }


        }

        /// <summary>
        /// Controla que los campos numericos sean correctos
        /// </summary>
        private void controlNumerico()
        {
            int edad1, edad2;

            if (int.TryParse(EntryEdadMaxima.Text.Trim(), out edad1) && int.TryParse(EntryEdadMinima.Text.Trim(), out edad2))
            {
                if (int.Parse(EntryEdadMinima.Text.ToString()) >= int.Parse(EntryEdadMaxima.Text.ToString()))
                {
                    //Mostrar advertencia
                    lanzarAdvertencia("La edad mínima no puede ser mayor o igual a la máxima.");
                }
                else { realizarBusqueda(); }
            }
            else
            {
                EntryEdadMaxima.Text = EntryEdadMinima.Text = "";
                lanzarAdvertencia("Ningun campo edad puede componerse por letras, solo aceptan valores numéricos.");
            }
        }

        /// <summary>
        /// Realiza la busqueda de verdad
        /// </summary>
        private void realizarBusqueda()
        {
            //Limpiamos los contactos cargados para volver a cargar los correctos
            contactosMostrar.Clear();
            //Se obtiene resultado de la busqueda con los valores introducidos y se carga en listView
            buscarContacto(EntryBuscarPorNombre.Text);
            //Se rellena listView
            cargarListView();
        }

        /// <summary>
        /// Vacia el listVew con los contactos buscados
        /// </summary>
        private void cargarListView()
        {
            lvwListaUsuarios.ItemsSource = contactosMostrar;
        }

        /// <summary>
        /// Implementa los filtros para obtener una list de contactos a mostrar
        /// </summary>
        /// <param name="filtro"></param>
        public void buscarContacto(string filtro)
        {

            /// Recorremos el array contactos y si encontramos una coincidencia la añadimos al arraylist resultado
            for (int i = 0; i < contactos.Count; i++)
            {
                if (comprobarNombre(contactos[i], filtro))
                {
                    contactosMostrar.Add(contactos[i]);
                }
            }

            //Si no se encontro ninguna coincidencia se informa
            if (contactosMostrar.Count == 0) { lanzarAdvertencia("No se encontro ninguna coincidencia, prueba de nuevo."); }
        }

        /// <summary>
        /// Metodo que comprueba si el nombre del contacto tiene alguna coincidencia con el filtro de busqueda.
        /// </summary>
        /// <param name="contacto">El contacto a analizar.</param>
        /// <param name="filtro">La cadena que queremos usar como filtro. Puede estar vacia. En ese caso, devolveremos siempre true.</param>
        /// <returns> Devuelve true si se ha encontrado coincidencia, devuelve false si no la encuentra.</returns>
        public Boolean comprobarNombre(Contacto contacto, string filtro)
        {
            Boolean ok = false;
            Regex rgx = new Regex("%", RegexOptions.IgnoreCase);

            matches = rgx.Matches(filtro);

            /// Primero tenemos que controlar que en el filtro no hemos introducido mas de un %.
            if (matches.Count > 1)
            {
                inputControl(EntryBuscarPorNombre, "No puede indicar más de un % en una busqueda.");
            }
            else
            {
                /// Si no se ha escrito nada en el patron de busqueda, o solo se ha escrito %...
                if (filtro.Trim().Equals("") || filtro.Trim().Equals("%"))
                {
                    if (EntryEdadMinima.Text.Trim().Length > 0 && EntryEdadMaxima.Text.Trim().Length == 0)
                    {
                        ok = comprobarEdad(contacto, EntryEdadMinima.Text.Trim(), true);
                    }
                    else if (EntryEdadMinima.Text.Trim().Length == 0 && EntryEdadMaxima.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, EntryEdadMaxima.Text.Trim(), false);
                    }
                    else if (EntryEdadMinima.Text.Trim().Length > 0 && EntryEdadMaxima.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, EntryEdadMinima.Text.Trim(), EntryEdadMaxima.Text.Trim());
                    }
                    else
                    {
                        ok = true;
                    }
                }
                /// Si el ultimo caracter es %...
                else if (filtro.Substring(filtro.Length - 1).Equals("%"))
                {
                    /// Quitamos el caracter % para poder usarlo como patron de busqueda.
                    filtro = filtro.Replace("%", "");
                    rgx = new Regex(String.Format("^" + filtro + ".*"), RegexOptions.IgnoreCase);
                    matches = rgx.Matches(contacto.Nombre);
                    /// Si encuentra alguna coincidencia, devolvemos true siempre y cuando la edad tambien coincida.
                    if (matches.Count > 0 && EntryEdadMinima.Text.Trim().Length > 0 && EntryEdadMaxima.Text.Trim().Length == 0)
                    {
                        ok = comprobarEdad(contacto, EntryEdadMinima.Text.Trim(), true);
                    }
                    else if (matches.Count > 0 && EntryEdadMinima.Text.Trim().Length == 0 && EntryEdadMaxima.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, EntryEdadMaxima.Text.Trim(), false);
                    }
                    else if (matches.Count > 0 && EntryEdadMinima.Text.Trim().Length > 0 && EntryEdadMaxima.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, EntryEdadMinima.Text.Trim(), EntryEdadMaxima.Text.Trim());
                    }
                    else if (matches.Count > 0)
                    {
                        ok = true;
                    }
                }
                /// Si en el patron de busqueda no hemos puesto como ultimo caracter un %...
                else
                {
                    rgx = new Regex("^" + filtro + "$", RegexOptions.IgnoreCase);
                    matches = rgx.Matches(contacto.Nombre);
                    /// Si encuentra alguna coincidencia, devolvemos true siempre y cuando la edad tambien coincida.
                    if (matches.Count > 0 && EntryEdadMinima.Text.Trim().Length > 0 && EntryEdadMaxima.Text.Trim().Length == 0)
                    {
                        ok = comprobarEdad(contacto, EntryEdadMinima.Text.Trim(), true);
                    }
                    else if (matches.Count > 0 && EntryEdadMinima.Text.Trim().Length == 0 && EntryEdadMaxima.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, EntryEdadMaxima.Text.Trim(), false);
                    }
                    else if (matches.Count > 0 && EntryEdadMinima.Text.Trim().Length > 0 && EntryEdadMaxima.Text.Trim().Length > 0)
                    {
                        ok = comprobarEdad(contacto, EntryEdadMinima.Text.Trim(), EntryEdadMaxima.Text.Trim());
                    }
                    else if (matches.Count > 0)
                    {
                        ok = true;
                    }
                }
            }

            return ok;
        }

        /// <summary>
        /// Metodo que compara la edad introducida en el formulario con la edad de un contacto.
        /// </summary>
        /// <param name="contacto">El contacto a analizar.</param>
        /// <param name="edad">La edad introducida al formulario para comparar.</param>
        /// <param name="modoMayor">Determina si estamos buscando mayores que la edad introducida o menos que la edad introducida</param>
        /// <returns>Devuelve true si el contacto tiene la edad correcta. Devuelve false si no cumple.</returns>
        public Boolean comprobarEdad(Contacto contacto, string edad, Boolean modoMayor)
        {
            Boolean ok = false;

            if ((Int32.Parse(contacto.Edad) < Int32.Parse(edad) && !modoMayor) || (Int32.Parse(contacto.Edad) >= Int32.Parse(edad) && modoMayor))
            {
                ok = true;
            }

            return ok;
        }

        /// <summary>
        /// Metodo que compara la edad introducida en el formulario con la edad de un contacto.
        /// </summary>
        /// <param name="contacto">El contacto a analizar.</param>
        /// <param name="edadMin">La edad minima introducida al formulario para comparar.</param>
        /// <param name="edadMax">La edad maxima introducida al formulario para comparar.</param>
        /// <returns>Devuelve true si el contacto tiene la edad correcta. Devuelve false si no cumple.</returns>
        public Boolean comprobarEdad(Contacto contacto, string edadMin, string edadMax)
        {
            Boolean ok = false;

            if (Int32.Parse(contacto.Edad) <= Int32.Parse(edadMax) && Int32.Parse(contacto.Edad) > Int32.Parse(edadMin))
            {
                ok = true;
            }

            return ok;
        }

        /// <summary>
        /// Muestra mensaje de error con mas de un %
        /// </summary>
        /// <param name="control"></param>
        /// <param name="mensaje"></param>
        private void inputControl(Entry control, String mensaje)
        {
            int edad;

            if (!int.TryParse(EntryEdadMaxima.Text.Trim(), out edad))
            {
                control.Text = "";
                //Mostrar advertencia
                lanzarAdvertencia(mensaje);
            }
        }
        #endregion    
    }
}
