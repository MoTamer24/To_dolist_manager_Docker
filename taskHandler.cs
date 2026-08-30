using Microsoft.AspNetCore.Authentication.OAuth.Claims;

public interface taskCrud
{
    abstract  status create(taskDTOCreate dto);
    // abstract  status delete();
    // abstract status edit();
    abstract  status all();
 
}

public class taskHandler:taskCrud{
    public taskHandler(dbContext db)
    {
        this.db=db;
    }
    private dbContext db;

    public status create(taskDTOCreate dto)
    {
        var task_new= new Task_todo();
        task_new.creationDate=DateTime.UtcNow;
        task_new.description=dto.description;
        task_new.taskName=dto.taskName;
        task_new.id=Guid.NewGuid();



        // save to db
        try{
        db.Tasks.Add(task_new);
        }
        catch(Exception e)
        {

            System.Console.WriteLine(e.Message);
            return new status(false,"db error");
        }


        return new status(true);
    }
    // public status delete()
    // {
        
    // }
    // public  status edit()
    // {
        
    // }
    public status all()
    {
        var load=db.Tasks;
        return new status(true,"list of tasks",load);
    }
}



public class taskDTOCreate
{
    public string? taskName{ get; set; }
    public string? description{get;set;}
   
}

public class status
{
    public status(bool good,string? msg=null,object? load=null)
    {
        this.good=good;
        this.msg=msg;
        this.load=load;
    }
    public bool good { get; set; }
    public string? msg { get; set; }=null;
    public object?load{get;set;}=null;
}