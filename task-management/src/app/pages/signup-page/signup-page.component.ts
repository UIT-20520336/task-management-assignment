import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../../user.service';

@Component({
  selector: 'app-signup-page',
  templateUrl: './signup-page.component.html',
  styleUrls: ['./signup-page.component.scss']
})
export class SignupPageComponent {
  signupForm!: FormGroup;
  isAccountCreated = false;
  isAccountCreationFailed = false;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private router: Router
  ) {
    this.signupForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.signupForm.valid && this.passwordsMatch()) {
      this.isAccountCreated = true;
      this.isAccountCreationFailed = false;
      const { username, password } = this.signupForm.value;
      this.userService.signup(username, password).subscribe({
        next: () => {
        }
      });
    } else {
      this.isAccountCreated = false;
      this.isAccountCreationFailed = true;
    }
  }

  private passwordsMatch(): boolean {
    const { password, confirmPassword } = this.signupForm.value;
    return password === confirmPassword;
  }
  cancel(): void {
    this.router.navigate(['/login']);
  }

}
