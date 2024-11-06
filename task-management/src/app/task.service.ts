import { Injectable } from '@angular/core';
import { WebRequestService } from './web-request.service';
import { Task } from './models/task.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  constructor(private webReqService: WebRequestService) { }

  createTask(title: string, description: string, isCompleted: boolean, userId: number): Observable<Task> {
    const task = {
      title: title,
      description: description,
      isCompleted: isCompleted,
      userId: userId
    };
    return this.webReqService.post('Tasks', task);
  }
  getTask(): Observable<Task[]> {
    return this.webReqService.get<Task[]>('Tasks');
  }
  updateTaskStatus(task: Task): Observable<Task> {
    return this.webReqService.patch(`Tasks/${task.id}`, {
      id: task.id,
      title: task.title,
      description: task.description,
      IsCompleted: !task.isCompleted
    });
  }
  deleteTask(id: number) {
    return this.webReqService.delete(`Tasks/${id}`);
  }

  updateTask(task: Task): Observable<Task> {
    return this.webReqService.patch<Task>(`Tasks/${task.id}`, task);
  }
}
