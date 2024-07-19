using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using To_Do_List;
using To_Do_List.Models;
using System.Linq;
using To_Do_List;
using Task = To_Do_List.Models.Task;
using To_Do_List.Service;

public class TasksController : Controller
{
    private readonly TaskContext _context;
    private readonly EmailService _emailService;

    public TasksController(TaskContext context , EmailService emailService)
    {
        _emailService = emailService;
        _context = context;
    }

    // GET: Tasks
    public IActionResult Index()
    {
        var tasks = _context.Tasks.ToList();
        return View(tasks);
    }

    // GET: Tasks/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Tasks/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Id,Title,Description,IsCompleted,Priority,DueDate,Category,Progress")] Task task)
    {
        if (ModelState.IsValid)
        {
            _context.Add(task);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(task);
    }

    // GET: Tasks/Edit/5
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var task = _context.Tasks.Find(id);
        if (task == null)
        {
            return NotFound();
        }
        return View(task);
    }

    // POST: Tasks/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("Id,Title,Description,IsCompleted,Priority,DueDate,Category,Progress")] Task task)
    {
        if (id != task.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(task);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(task.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(task);
    }

    // GET: Tasks/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var task = _context.Tasks
            .FirstOrDefault(m => m.Id == id);
        if (task == null)
        {
            return NotFound();
        }

        return View(task);
    }

    // POST: Tasks/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var task = _context.Tasks.Find(id);
        _context.Tasks.Remove(task);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    private bool TaskExists(int id)
    {
        return _context.Tasks.Any(e => e.Id == id);
    }

    [HttpPost]
    public IActionResult UpdateIsCompleted(int id, bool isCompleted)
    {
        var task = _context.Tasks.Find(id);
        if (task == null)
        {
            return Json(new { success = false });
        }

        task.IsCompleted = isCompleted;
        _context.SaveChanges();
        if (isCompleted)
        {
            // Replace with the recipient's email address
            _emailService.SendEmail("manev.yavor@gmail.com", $"Task Completed: {task.Title}", $"The task '{task.Title}' has been marked as completed. Good JOB!");
        }
        return Json(new { success = true });
    }
}
