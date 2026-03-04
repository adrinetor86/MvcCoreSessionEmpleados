using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;

namespace MvcCoreSessionEmpleados.Controllers;

public class EmpleadosController : Controller
{
    private RepositoryEmpleados _repo;
    private IMemoryCache _memoryCache;
    public EmpleadosController(RepositoryEmpleados repo,IMemoryCache memoryCache)
    {
        _repo = repo;
        _memoryCache = memoryCache;
    }

    // GET
    public async Task<IActionResult> SessionSalarios(int? salario)
    {
        if (salario != null)
        {
            //QUEREMOS ALMACENAR LA SUMA TOTAL DE SALARIOS
            //QUE TENGAMOS EN SESSION
            int sumaTotal = 0;
            if (HttpContext.Session.GetString("SUMASALARIAL") != null)
            {
                //SI YA TENEMOS DATOS ALMACENADOS, LOS RECUPERAMOS 
                sumaTotal = HttpContext.Session.GetObject<int>("SUMASALARIAL");
            }

            //SUMAMOS EL NUEVO SALARIO A LA SUMA TOTAL
            sumaTotal += salario.Value;
            //ALMACENAMOS EL VALOR DENTRO DE SESSION
            HttpContext.Session.SetObject("SUMASALARIAL", sumaTotal);
            ViewData["MENSAJE"] = "Salario almacenado: " + salario;
        }

        List<Empleado> empleados = await _repo.GetEmpleadosAsync();

        return View(empleados);
    }

    public async Task<IActionResult> SessionEmpleados(int? idempleado)
    {
        if (idempleado != null)
        {
            Empleado empleado = await _repo.FindEmpleadoAsync(idempleado.Value);
            //EN SESSION TENDREMOS ALMACENADOS UN CONJUNTO DE EMPLEADOS
            List<Empleado> empleadosList;
            //DEBEMOS PREGUNTAR SI YA TENEMOS EMPLEADOS EN SESSION
            if (HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS") != null)
            {
                //RECUPERAMOS LA VISTA DE SESSION
                empleadosList = HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS");
            }
            else
            {
                //CREAMOS UNA NUEVA LISTA PARA ALMACENAR LOS EMPLEADOS
                empleadosList = new List<Empleado>();
            }

            //AGREGAMOS EL EMPLEADO A LIST
            empleadosList.Add(empleado);
            //ALMACENAMOS LA VISTA EN SESSION
            HttpContext.Session.SetObject("EMPLEADOS", empleadosList);
            ViewData["MENSAJE"] = "Empleado " + empleado.Apellido + " almacenado correctamente";
        }

        List<Empleado> empleados = await _repo.GetEmpleadosAsync();
        return View(empleados);
    }

    public async Task<IActionResult> SessionEmpleadosOk(int? idEmpleado)
    {
        if (idEmpleado != null)
        {
            //ALMACENAMOS LO MINIMO...
            List<int> idsEmpleados;

            if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") != null)
            {
                //RECUPERAMOS LA COLECCION
                idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            }
            else
            {
                //CREAMOS LA COLECCION
                idsEmpleados = new List<int>();
            }

            //ALMACENAMOS EL ID DEL EMPLEADO
            idsEmpleados.Add(idEmpleado.Value);
            HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
            ViewData["MENSAJE"] = "Empleados almacenados: " + idsEmpleados.Count;
        }

        List<Empleado> empleados = await _repo.GetEmpleadosAsync();
        return View(empleados);
    }


    public async Task<IActionResult> EmpleadosAlmacenadosOk()
    {
        List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");

        if (idsEmpleados == null)
        {
            ViewData["MENSAJE"] = "No existen empleados en Session";
            return View();
        }
        else
        {
            List<Empleado> empleados = await _repo.GetEmpleadosSessionAsync(idsEmpleados);
            return View(empleados);
        }
    }

    public async Task<IActionResult> SessionEmpleadosV4(int? idEmpleado)
    {
        if (idEmpleado != null)
        {
            //ALMACENAMOS LO MINIMO...
            List<int> idsEmpleados;

            if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOSV4") != null)
            {
                //RECUPERAMOS LA COLECCION
                idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOSV4");
            }
            else
            {
                //CREAMOS LA COLECCION
                idsEmpleados = new List<int>();
            }
            //ALMACENAMOS EL ID DEL EMPLEADO
            idsEmpleados.Add(idEmpleado.Value);
            HttpContext.Session.SetObject("IDSEMPLEADOSV4", idsEmpleados);
            ViewData["MENSAJE"] = "Empleados almacenados: " + idsEmpleados.Count;
        }

        List<int> ids = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOSV4");
        if (ids != null)
        {
            List<Empleado> empleados = await _repo.GetEmpleadosSinSessionAsync(ids);
            return View(empleados);
        }
        else
        {
            List<Empleado> empleados = await _repo.GetEmpleadosAsync();
            return View(empleados);
        }

    }


    public async Task<IActionResult> EmpleadosAlmacenadosV4()
    {
        List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOSV4");

        if (idsEmpleados == null)
        {
            ViewData["MENSAJE"] = "No existen empleados en Session";
            return View();
        }
        else
        {
            List<Empleado> empleados = await _repo.GetEmpleadosSessionAsync(idsEmpleados);
            return View(empleados);
        }
    }
    

    public async Task<IActionResult> SessionEmpleadosV5
        (int? idempleado,int? idfavorito)
    {
        if (idfavorito != null)
        {
            //COMO ESTOY GUARDANDO EN CACHE, VAMOS A GUARDAR
            //DIRECTAMENTE LOS OBJETOS EN LUGAR DE LOS IDS

            List<Empleado> empleadosFavoritos;
            if (_memoryCache.Get("FAVORITOS") == null)
            {
                //NO EXISTE NADA EN CACHE

                empleadosFavoritos = new List<Empleado>();

            }
            else
            {
                //RECUPERAMOS EL CACHE
                empleadosFavoritos= _memoryCache.Get<List<Empleado>>("FAVORITOS");
            }
            
            
            //BUSCAMOS AL EMPLEADO PARA GUARDARLO
            Empleado empleadoFavorito = await _repo.FindEmpleadoAsync(idfavorito.Value);
            empleadosFavoritos.Add(empleadoFavorito);
            _memoryCache.Set("FAVORITOS", empleadosFavoritos);
        }
        
        if (idempleado != null)
        {
            //ALMACENAMOS LO MINIMO...
            List<int> idsEmpleadosList;
            if (HttpContext.Session.GetObject<List<int>>
                    ("IDSEMPLEADOS") != null)
            {
                //RECUPERAMOS LA COLECCION
                idsEmpleadosList =
                    HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            }
            else
            {
                //CREAMOS LA COLECCION
                idsEmpleadosList = new List<int>();
            }
            //ALMACENAMOS EL ID DEL EMPLEADO
            idsEmpleadosList.Add(idempleado.Value);
            //ALMACENAMOS EN SESSION LOS DATOS
            HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleadosList);
            ViewData["MENSAJE"] = "Empleados almacenados: "
                                  + idsEmpleadosList.Count;
        }
        
        List<Empleado> empleados = await _repo.GetEmpleadosAsync();
        return View(empleados);
    }

    public IActionResult EmpleadosFavoritos()
    {
        return View();
    }
  
    public async Task<IActionResult> EmpleadosAlmacenadosV5(int? ideliminar)
    {
        //RECUPERAMOS LA COLECCION DE SESSION
        List<int> idsEmpleados =
            HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
        if (idsEmpleados == null)
        {
            ViewData["MENSAJE"] = "No existen empleados en Session";
            return View();
        }
        else
        {
            //PREGUNTAMOS SI HEMOS RECIBIDO ALGUN DATO PARA ELIMINAR

            if (ideliminar != null)
            {
                idsEmpleados.Remove(ideliminar.Value);
                

                if (idsEmpleados.Count == 0)
                {
                    HttpContext.Session.Remove("IDSEMPLEADOS");
                }
                else
                {
                    //ACTUALIZAMOS SESSION
                    HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                    
                }
            }
            List<Empleado> empleados =
                await _repo.GetEmpleadosSessionAsync(idsEmpleados);
            return View(empleados);
        }
    }
    


    public IActionResult EmpleadosAlmacenados()
    {
        return View();
    }

    public IActionResult SumaSalarial()
    {
        return View();
    }

    public IActionResult Index()
    {
        return View();
    }
}