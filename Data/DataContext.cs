using Microsoft.EntityFrameworkCore;
using MvcCoreSessionEmpleados.Models;

namespace MvcCoreSessionEmpleados.Data;

public class DataContext :DbContext
{
    
    public DataContext(DbContextOptions<DataContext> options):base(options){}
    
    public DbSet<Empleado> Empleados { get; set; }
}