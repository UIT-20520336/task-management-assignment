import { Component,OnInit } from '@angular/core';
import { TaskService } from '../../task.service';
import { ActivatedRoute, Params } from '@angular/router';
import { Task } from '../../models/task.model';
import { UserService } from '../../user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-task-view',
  templateUrl: './task-view.component.html',
  styleUrls: ['./task-view.component.scss']
})
export class TaskViewComponent implements OnInit {
  constructor(private taskService: TaskService, private route: ActivatedRoute, private userService: UserService, private router: Router) { }
  tasks: Task[] = [];
  filteredTasks: Task[] = [];
  filter: string = 'all';
  editModalOpen = false;
  editTaskData: Task = { id: 0, title: '', description: '', isCompleted: false, userId: 0 };

  ngOnInit() {
    this.route.params.subscribe(
      (params: Params) => {
        console.log(params);
      }
    )
    this.getTasks();
  }

  getTasks() {
    this.taskService.getTask().subscribe((tasks: Task[]) => {
      this.tasks = tasks;
      this.filteredTasks = tasks;
      this.applyFilter();
    });
  }


  createNewTask() {
    const newTask = {
      title: 'New Task Title',
      description: 'New Task Description',
      isCompleted: false,
      userId: this.userService.getCurrentUserId()
    };
    this.taskService.createTask(newTask.title, newTask.description, newTask.isCompleted, newTask.userId as number)
      .subscribe({
        next: (response) => {
          this.getTasks();
          this.applyFilter();
        },
        error: (err) => {
          console.error('Error creating task:', err);
          
        }
      });
  }

  toggleTaskStatus(task: Task) {
    this.taskService.updateTaskStatus(task).subscribe(() => {
      task.isCompleted = !task.isCompleted;
      
    })
  }

  deleteTask(id: number) {
    this.taskService.deleteTask(id).subscribe(() => {
      this.tasks = this.tasks.filter(task => task.id !== id);
      this.getTasks();
      this.applyFilter();
    });
  }

  openEditModal(task: any) {
    this.editModalOpen = true;
    this.editTaskData = { ...task };
  }

  closeEditModal() {
    this.editModalOpen = false;
    this.editTaskData = { id: 0, title: '', description: '', isCompleted: false, userId: 0 };
  }
  submitEdit() {
    this.taskService.updateTask(this.editTaskData).subscribe( updatedTask => {
      const index = this.tasks.findIndex(task => task.id === updatedTask.id);
      if (index !== -1) {
        this.tasks[index] = updatedTask;
      }
      this.closeEditModal();
      this.getTasks();
      this.applyFilter();
    });
  }
  onFilterChange(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    this.filter = selectElement.value;
    this.applyFilter();
  }
  applyFilter() {
    if (this.filter === 'completed') {
      this.filteredTasks = this.tasks.filter(task => task.isCompleted);
    } else if (this.filter === 'pending') {
      this.filteredTasks = this.tasks.filter(task => !task.isCompleted);
    } else {
      this.filteredTasks = this.tasks;
    }
  }

  logout() {
    this.userService.logout();
    this.router.navigate(['/login']);
  }
}
