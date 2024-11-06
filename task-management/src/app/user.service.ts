import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { jwtDecode } from 'jwt-decode';


@Injectable({
  providedIn: 'root'
})
export class UserService {
  private ROOT_URL = environment.apiBaseUrl;
  private TOKEN_KEY = 'token';
  public isAuthenticated = new BehaviorSubject<boolean>(this.isLoggedIn());
  constructor(private http: HttpClient) { }

  login(username: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.ROOT_URL}/Users/login`, { username, password }).pipe(
      tap((response: any) => {
        this.setToken(response.token);
        this.isAuthenticated.next(true);
      })
    );
  }

  signup(username: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.ROOT_URL}/Users/register`, { username, password });
  }


  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }
  private setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }
  getCurrentUserId(): number | null {
    const token = this.getToken();
    if (token) {
      try {
        const decodedToken: any = jwtDecode(token);
        return decodedToken.userId;
      } catch (error) {
        console.error("Failed to decode token", error);
        return null;
      }
    }
    return null;
  }

}
