import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root'
})
export class WebRequestService {

  readonly ROOT_URL;

  constructor(private http: HttpClient, private userService: UserService) {
    this.ROOT_URL = environment.apiBaseUrl;
    
  }
  
  get<T>(uri: string): Observable<T> {
    const headers = this.userService.getAuthHeaders();
    return this.http.get<T>(`${this.ROOT_URL}/${uri}`, { headers });
  }

  post<T>(uri: string, payload: Object): Observable<T> {
    const headers = this.userService.getAuthHeaders();
    return this.http.post<T>(`${this.ROOT_URL}/${uri}`, payload, { headers });
  }

  patch<T>(uri: string, payload: Object): Observable<T> {
    const headers = this.userService.getAuthHeaders();
    return this.http.patch<T>(`${this.ROOT_URL}/${uri}`, payload, { headers});
  }

  delete<T>(uri: string): Observable<T> {
    const headers = this.userService.getAuthHeaders();
    return this.http.delete<T>(`${this.ROOT_URL}/${uri}`, { headers });
  }


}
