using Microsoft.EntityFrameworkCore;
using MvcCoreSessionEmpleados.Data;
using MvcCoreSessionEmpleados.Models;

namespace MvcCoreSessionEmpleados.Repositories;

public class RepositoryEmpleados
{
    private DataContext _context;

    public RepositoryEmpleados(DataContext context)
    {
        _context = context;
    }

    public async Task<List<Empleado>> GetEmpleadosAsync()
    {
        var consulta = from datos in _context.Empleados
            select datos;

        return await consulta.ToListAsync();
    } 
    
    public async Task<Empleado> FindEmpleadoAsync(int idEmpleado)
    {
        var consulta = from datos in _context.Empleados
            where datos.IdEmpleado == idEmpleado
            select datos;

        return await consulta.FirstOrDefaultAsync();
    }

    public async Task<List<Empleado>> GetEmpleadosSessionAsync(List<int> idsEmpleados)
    {
        var consulta = from datos in _context.Empleados
                where idsEmpleados.Contains(datos.IdEmpleado)
                select datos;
        
        return await consulta.ToListAsync();
    }
    
    public async Task<List<Empleado>> GetEmpleadosSinSessionAsync(List<int> idsEmpleados)
    {
        var consulta = from datos in _context.Empleados
            where !idsEmpleados.Contains(datos.IdEmpleado)
            select datos;

        return await consulta.ToListAsync();
    } 

}