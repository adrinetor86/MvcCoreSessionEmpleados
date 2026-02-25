using Microsoft.AspNetCore.Mvc;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;

namespace MvcCoreSessionEmpleados.Controllers;

public class EmpleadosController : Controller
{
    private RepositoryEmpleados _repo;

    public EmpleadosController(RepositoryEmpleados repo)
    {
        _repo = repo;
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