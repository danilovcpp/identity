# Next.js Frontend Integration Prompt

## Project Context

You need to build a Next.js frontend application that integrates with an ASP.NET Core Identity API. The API provides JWT-based authentication with refresh tokens, user management, email confirmation, password reset, profile management, and avatar upload functionality.

## API Details

**Base URL:** `http://localhost:5000` (development)

**Authentication:** JWT Bearer tokens with refresh token support

**Full API Specification:** See [API_SPECIFICATION.md](./API_SPECIFICATION.md) for complete endpoint documentation.

---

## Technical Requirements

### Framework & Libraries

**Core:**
- Next.js 15+ (App Router)
- React 18+
- TypeScript

**Recommended Libraries:**
- **State Management:** Zustand or React Context
- **HTTP Client:** Axios or native fetch with wrapper
- **Form Management:** React Hook Form + Zod validation
- **UI Components:** shadcn/ui, Radix UI, or MUI
- **Styling:** Tailwind CSS
- **Token Storage:** js-cookie or localStorage with encryption

### Project Structure

```
src/
├── app/
│   ├── (auth)/
│   │   ├── login/
│   │   ├── register/
│   │   ├── forgot-password/
│   │   ├── reset-password/
│   │   └── confirm-email/
│   ├── (protected)/
│   │   ├── profile/
│   │   ├── settings/
│   │   └── dashboard/
│   └── layout.tsx
├── components/
│   ├── auth/
│   │   ├── LoginForm.tsx
│   │   ├── RegisterForm.tsx
│   │   └── ProtectedRoute.tsx
│   ├── profile/
│   │   ├── AvatarUpload.tsx
│   │   └── ProfileCard.tsx
│   └── ui/ (shadcn components)
├── lib/
│   ├── api/
│   │   ├── client.ts (Axios/fetch wrapper)
│   │   ├── auth.ts
│   │   ├── profile.ts
│   │   └── avatar.ts
│   ├── auth/
│   │   ├── authStore.ts (Zustand store)
│   │   ├── tokenManager.ts
│   │   └── authHelpers.ts
│   ├── types/
│   │   ├── auth.ts
│   │   ├── user.ts
│   │   └── api.ts
│   └── utils/
│       ├── validators.ts (Zod schemas)
│       └── errors.ts
├── hooks/
│   ├── useAuth.ts
│   ├── useProfile.ts
│   └── useTokenRefresh.ts
└── middleware.ts (Route protection)
```

---

## Implementation Guide

### 1. API Client Setup

Create a base API client with automatic token refresh:

```typescript
// lib/api/client.ts
import axios, { AxiosInstance, AxiosError } from 'axios';
import { getAccessToken, getRefreshToken, setTokens, clearTokens } from '@/lib/auth/tokenManager';
import { refreshAccessToken } from '@/lib/api/auth';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';

export const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor - add access token
apiClient.interceptors.request.use(
  (config) => {
    const token = getAccessToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor - handle token refresh
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config;

    // If 401 and we haven't retried yet
    if (error.response?.status === 401 && !originalRequest?._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = getRefreshToken();
        if (!refreshToken) {
          throw new Error('No refresh token');
        }

        const { accessToken, refreshToken: newRefreshToken } = await refreshAccessToken(refreshToken);
        setTokens(accessToken, newRefreshToken);

        // Retry original request with new token
        if (originalRequest) {
          originalRequest.headers.Authorization = `Bearer ${accessToken}`;
          return apiClient(originalRequest);
        }
      } catch (refreshError) {
        // Refresh failed - logout user
        clearTokens();
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);
```

**Key Features:**
- Automatic token injection
- Automatic token refresh on 401
- Error handling and logout on refresh failure

---

### 2. Token Management

```typescript
// lib/auth/tokenManager.ts
import Cookies from 'js-cookie';

const ACCESS_TOKEN_KEY = 'access_token';
const REFRESH_TOKEN_KEY = 'refresh_token';

export const setTokens = (accessToken: string, refreshToken: string) => {
  // Store access token in memory or sessionStorage (short-lived)
  sessionStorage.setItem(ACCESS_TOKEN_KEY, accessToken);

  // Store refresh token in httpOnly cookie (if backend supports) or secure cookie
  Cookies.set(REFRESH_TOKEN_KEY, refreshToken, {
    expires: 7, // 7 days
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'strict',
  });
};

export const getAccessToken = (): string | null => {
  return sessionStorage.getItem(ACCESS_TOKEN_KEY);
};

export const getRefreshToken = (): string | null => {
  return Cookies.get(REFRESH_TOKEN_KEY) || null;
};

export const clearTokens = () => {
  sessionStorage.removeItem(ACCESS_TOKEN_KEY);
  Cookies.remove(REFRESH_TOKEN_KEY);
};

// Decode JWT to get user info (without verification - for client-side only)
export const decodeToken = (token: string) => {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch {
    return null;
  }
};
```

---

### 3. Authentication Store (Zustand)

```typescript
// lib/auth/authStore.ts
import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { decodeToken } from './tokenManager';

interface User {
  id: string;
  email: string;
  userName?: string;
  avatarUrl?: string;
}

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  setUser: (user: User | null) => void;
  setLoading: (loading: boolean) => void;
  logout: () => void;
  initializeAuth: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      isAuthenticated: false,
      isLoading: true,

      setUser: (user) => set({
        user,
        isAuthenticated: !!user,
        isLoading: false
      }),

      setLoading: (loading) => set({ isLoading: loading }),

      logout: () => {
        clearTokens();
        set({ user: null, isAuthenticated: false });
      },

      initializeAuth: () => {
        const token = getAccessToken();
        if (token) {
          const decoded = decodeToken(token);
          if (decoded && decoded.exp * 1000 > Date.now()) {
            set({
              user: {
                id: decoded.sub,
                email: decoded.email,
              },
              isAuthenticated: true,
              isLoading: false,
            });
            return;
          }
        }
        set({ isLoading: false });
      },
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({ user: state.user }), // Only persist user
    }
  )
);
```

---

### 4. API Service Functions

```typescript
// lib/api/auth.ts
import { apiClient } from './client';
import { setTokens, clearTokens } from '@/lib/auth/tokenManager';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

export interface RegisterRequest {
  email: string;
  password: string;
}

export const authApi = {
  async login(data: LoginRequest): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>('/api/login', data);
    setTokens(response.data.accessToken, response.data.refreshToken);
    return response.data;
  },

  async register(data: RegisterRequest) {
    const response = await apiClient.post('/api/register', data);
    return response.data;
  },

  async refreshAccessToken(refreshToken: string): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>('/api/refresh', {
      refreshToken,
    });
    return response.data;
  },

  async logout(refreshToken: string) {
    try {
      await apiClient.post('/api/revoke', { refreshToken });
    } finally {
      clearTokens();
    }
  },

  async forgotPassword(email: string) {
    const response = await apiClient.post('/api/forgot-password', { email });
    return response.data;
  },

  async resetPassword(email: string, token: string, newPassword: string) {
    const response = await apiClient.post('/api/reset-password', {
      email,
      token,
      newPassword,
    });
    return response.data;
  },

  async changePassword(currentPassword: string, newPassword: string) {
    const response = await apiClient.post('/api/change-password', {
      currentPassword,
      newPassword,
    });
    return response.data;
  },

  async confirmEmail(userId: string, token: string) {
    const response = await apiClient.get('/api/confirm-email', {
      params: { userId, token },
    });
    return response.data;
  },
};
```

```typescript
// lib/api/profile.ts
import { apiClient } from './client';

export interface UserProfile {
  id: string;
  email: string;
  userName?: string;
  avatarUrl?: string;
  emailConfirmed: boolean;
  createdAt: string;
}

export const profileApi = {
  async getProfile(userId: string): Promise<UserProfile> {
    const response = await apiClient.get<UserProfile>(`/api/profile/${userId}`);
    return response.data;
  },

  async getMyProfile(): Promise<UserProfile> {
    const response = await apiClient.get<UserProfile>('/api/profile/me');
    return response.data;
  },
};
```

```typescript
// lib/api/avatar.ts
import { apiClient } from './client';

export interface AvatarUploadResponse {
  avatarUrl: string;
  message: string;
}

export const avatarApi = {
  async uploadAvatar(file: File): Promise<AvatarUploadResponse> {
    const formData = new FormData();
    formData.append('file', file);

    const response = await apiClient.post<AvatarUploadResponse>(
      '/api/avatar/upload',
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    return response.data;
  },

  async deleteAvatar(): Promise<void> {
    await apiClient.delete('/api/avatar');
  },
};
```

---

### 5. Form Validation Schemas (Zod)

```typescript
// lib/utils/validators.ts
import { z } from 'zod';

export const loginSchema = z.object({
  email: z.string().email('Invalid email address'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
});

export const registerSchema = z.object({
  email: z.string().email('Invalid email address'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
  confirmPassword: z.string(),
}).refine((data) => data.password === data.confirmPassword, {
  message: "Passwords don't match",
  path: ['confirmPassword'],
});

export const changePasswordSchema = z.object({
  currentPassword: z.string().min(1, 'Current password is required'),
  newPassword: z.string().min(6, 'Password must be at least 6 characters'),
  confirmPassword: z.string(),
}).refine((data) => data.newPassword === data.confirmPassword, {
  message: "Passwords don't match",
  path: ['confirmPassword'],
});

export const resetPasswordSchema = z.object({
  newPassword: z.string().min(6, 'Password must be at least 6 characters'),
  confirmPassword: z.string(),
}).refine((data) => data.newPassword === data.confirmPassword, {
  message: "Passwords don't match",
  path: ['confirmPassword'],
});

export const forgotPasswordSchema = z.object({
  email: z.string().email('Invalid email address'),
});

export const avatarSchema = z.object({
  file: z
    .instanceof(File)
    .refine((file) => file.size <= 5 * 1024 * 1024, 'File size must be less than 5MB')
    .refine(
      (file) => ['image/jpeg', 'image/png', 'image/gif', 'image/webp'].includes(file.type),
      'File must be JPEG, PNG, GIF, or WebP'
    ),
});
```

---

### 6. React Components

#### Login Form Component

```typescript
// components/auth/LoginForm.tsx
'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { loginSchema } from '@/lib/utils/validators';
import { authApi } from '@/lib/api/auth';
import { useAuthStore } from '@/lib/auth/authStore';
import { useRouter } from 'next/navigation';
import { useState } from 'react';

type LoginFormData = {
  email: string;
  password: string;
};

export default function LoginForm() {
  const router = useRouter();
  const setUser = useAuthStore((state) => state.setUser);
  const [error, setError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    try {
      setError(null);
      const response = await authApi.login(data);

      // Decode token to get user info
      const decoded = decodeToken(response.accessToken);
      setUser({
        id: decoded.sub,
        email: decoded.email,
      });

      router.push('/dashboard');
    } catch (err: any) {
      if (err.response?.status === 401) {
        setError('Invalid email or password');
      } else if (err.response?.status === 403) {
        setError('Please confirm your email before logging in');
      } else {
        setError('An error occurred. Please try again.');
      }
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div>
        <label htmlFor="email">Email</label>
        <input
          id="email"
          type="email"
          {...register('email')}
          className="w-full px-3 py-2 border rounded"
        />
        {errors.email && (
          <p className="text-red-500 text-sm">{errors.email.message}</p>
        )}
      </div>

      <div>
        <label htmlFor="password">Password</label>
        <input
          id="password"
          type="password"
          {...register('password')}
          className="w-full px-3 py-2 border rounded"
        />
        {errors.password && (
          <p className="text-red-500 text-sm">{errors.password.message}</p>
        )}
      </div>

      {error && <p className="text-red-500">{error}</p>}

      <button
        type="submit"
        disabled={isSubmitting}
        className="w-full bg-blue-500 text-white py-2 rounded hover:bg-blue-600 disabled:opacity-50"
      >
        {isSubmitting ? 'Logging in...' : 'Login'}
      </button>
    </form>
  );
}
```

#### Avatar Upload Component

```typescript
// components/profile/AvatarUpload.tsx
'use client';

import { useState, useRef } from 'react';
import { avatarApi } from '@/lib/api/avatar';
import { useAuthStore } from '@/lib/auth/authStore';
import Image from 'next/image';

export default function AvatarUpload() {
  const { user, setUser } = useAuthStore();
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    // Validate file
    if (file.size > 5 * 1024 * 1024) {
      setError('File size must be less than 5MB');
      return;
    }

    const validTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];
    if (!validTypes.includes(file.type)) {
      setError('File must be JPEG, PNG, GIF, or WebP');
      return;
    }

    try {
      setError(null);
      setUploading(true);
      const response = await avatarApi.uploadAvatar(file);

      // Update user in store
      if (user) {
        setUser({ ...user, avatarUrl: response.avatarUrl });
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to upload avatar');
    } finally {
      setUploading(false);
    }
  };

  const handleDelete = async () => {
    try {
      setError(null);
      await avatarApi.deleteAvatar();

      if (user) {
        setUser({ ...user, avatarUrl: undefined });
      }
    } catch (err: any) {
      setError('Failed to delete avatar');
    }
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-4">
        {user?.avatarUrl ? (
          <Image
            src={user.avatarUrl}
            alt="Avatar"
            width={100}
            height={100}
            className="rounded-full"
          />
        ) : (
          <div className="w-24 h-24 bg-gray-200 rounded-full flex items-center justify-center">
            <span className="text-gray-500">No avatar</span>
          </div>
        )}

        <div className="space-x-2">
          <button
            onClick={() => fileInputRef.current?.click()}
            disabled={uploading}
            className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 disabled:opacity-50"
          >
            {uploading ? 'Uploading...' : 'Upload Avatar'}
          </button>

          {user?.avatarUrl && (
            <button
              onClick={handleDelete}
              className="px-4 py-2 bg-red-500 text-white rounded hover:bg-red-600"
            >
              Delete
            </button>
          )}
        </div>

        <input
          ref={fileInputRef}
          type="file"
          accept="image/jpeg,image/png,image/gif,image/webp"
          onChange={handleFileChange}
          className="hidden"
        />
      </div>

      {error && <p className="text-red-500">{error}</p>}
    </div>
  );
}
```

---

### 7. Route Protection Middleware

```typescript
// middleware.ts
import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export function middleware(request: NextRequest) {
  const token = request.cookies.get('refresh_token')?.value;
  const isAuthPage = request.nextUrl.pathname.startsWith('/login') ||
                     request.nextUrl.pathname.startsWith('/register');
  const isProtectedPage = request.nextUrl.pathname.startsWith('/dashboard') ||
                          request.nextUrl.pathname.startsWith('/profile') ||
                          request.nextUrl.pathname.startsWith('/settings');

  // Redirect to login if accessing protected page without token
  if (isProtectedPage && !token) {
    return NextResponse.redirect(new URL('/login', request.url));
  }

  // Redirect to dashboard if accessing auth page with token
  if (isAuthPage && token) {
    return NextResponse.redirect(new URL('/dashboard', request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ['/dashboard/:path*', '/profile/:path*', '/settings/:path*', '/login', '/register'],
};
```

---

### 8. Custom Hooks

```typescript
// hooks/useAuth.ts
import { useAuthStore } from '@/lib/auth/authStore';
import { useEffect } from 'react';

export function useAuth() {
  const { user, isAuthenticated, isLoading, logout, initializeAuth } = useAuthStore();

  useEffect(() => {
    initializeAuth();
  }, []);

  return {
    user,
    isAuthenticated,
    isLoading,
    logout,
  };
}
```

```typescript
// hooks/useProfile.ts
import { useQuery } from '@tanstack/react-query';
import { profileApi } from '@/lib/api/profile';

export function useProfile() {
  return useQuery({
    queryKey: ['profile', 'me'],
    queryFn: () => profileApi.getMyProfile(),
    staleTime: 5 * 60 * 1000, // 5 minutes
  });
}
```

---

## Environment Variables

Create `.env.local`:

```env
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_APP_URL=http://localhost:3000
```

---

## Key Implementation Notes

### 1. Token Storage Strategy
- **Access Token:** sessionStorage (cleared on tab close)
- **Refresh Token:** Secure cookie (httpOnly if possible, sameSite=strict)
- **User Data:** Zustand store with persistence

### 2. Automatic Token Refresh
- Implemented in Axios interceptor
- Triggers on 401 response
- Retries original request after refresh
- Logs out user if refresh fails

### 3. Error Handling
- Display user-friendly messages
- Handle 401 (Unauthorized), 403 (Forbidden), 400 (Validation)
- Email confirmation required error (403)
- Form validation errors

### 4. Security Best Practices
- Never log tokens to console in production
- Use HTTPS in production
- Implement CSRF protection
- Validate file uploads client-side
- Set secure cookie attributes
- Implement rate limiting on API

### 5. User Experience
- Loading states for all async operations
- Error messages for failed operations
- Success notifications
- Redirect after successful login/registration
- Remember user preferences
- Auto-logout on token expiration

### 6. Avatar Upload
- Client-side validation (type, size)
- Preview before upload
- Progress indicator
- Error handling
- Delete functionality

### 7. Email Confirmation Flow
- Show message after registration
- Handle confirmation link clicks
- Display success/error messages
- Redirect to login after confirmation

### 8. Password Reset Flow
- Request reset from login page
- Handle reset link clicks
- Form to enter new password
- Success message and redirect to login

---

## Testing Checklist

- [ ] User registration and email confirmation
- [ ] User login with valid/invalid credentials
- [ ] Email not confirmed error handling
- [ ] Token refresh on 401
- [ ] Logout functionality
- [ ] Password change (authenticated)
- [ ] Password reset flow
- [ ] Profile viewing (public and own)
- [ ] Avatar upload (valid file)
- [ ] Avatar upload error handling (size, type)
- [ ] Avatar deletion
- [ ] Protected route access without auth
- [ ] Form validation on all forms
- [ ] Error message display
- [ ] Loading states

---

## Additional Recommendations

1. **Use React Query/TanStack Query** for data fetching and caching
2. **Implement Toast Notifications** (react-hot-toast or sonner)
3. **Add Loading Skeletons** for better UX
4. **Implement Dark Mode** support
5. **Add Form Loading States** to prevent double submissions
6. **Implement Logout Everywhere** (revoke all refresh tokens)
7. **Add Profile Update** functionality (username, etc.)
8. **Implement Avatar Crop Tool** (react-easy-crop)
9. **Add Password Strength Indicator**
10. **Implement Email Resend** functionality

---

## Example Pages

### Login Page (`app/(auth)/login/page.tsx`)

```typescript
import LoginForm from '@/components/auth/LoginForm';
import Link from 'next/link';

export default function LoginPage() {
  return (
    <div className="min-h-screen flex items-center justify-center">
      <div className="max-w-md w-full space-y-8 p-8 bg-white rounded-lg shadow">
        <h1 className="text-2xl font-bold text-center">Login</h1>
        <LoginForm />
        <div className="text-center space-y-2">
          <Link href="/register" className="text-blue-500 hover:underline">
            Don't have an account? Register
          </Link>
          <br />
          <Link href="/forgot-password" className="text-blue-500 hover:underline">
            Forgot password?
          </Link>
        </div>
      </div>
    </div>
  );
}
```

### Dashboard Page (`app/(protected)/dashboard/page.tsx`)

```typescript
'use client';

import { useProfile } from '@/hooks/useProfile';
import AvatarUpload from '@/components/profile/AvatarUpload';

export default function DashboardPage() {
  const { data: profile, isLoading } = useProfile();

  if (isLoading) return <div>Loading...</div>;

  return (
    <div className="container mx-auto p-8">
      <h1 className="text-3xl font-bold mb-6">Dashboard</h1>

      <div className="bg-white rounded-lg shadow p-6 space-y-4">
        <h2 className="text-xl font-semibold">Profile</h2>

        <div>
          <p><strong>Email:</strong> {profile?.email}</p>
          <p><strong>Username:</strong> {profile?.userName || 'Not set'}</p>
          <p><strong>Email Confirmed:</strong> {profile?.emailConfirmed ? 'Yes' : 'No'}</p>
          <p><strong>Member Since:</strong> {new Date(profile?.createdAt || '').toLocaleDateString()}</p>
        </div>

        <AvatarUpload />
      </div>
    </div>
  );
}
```

---

## Summary

This prompt provides everything needed to build a complete Next.js frontend that integrates with your ASP.NET Core Identity API. The implementation includes:

- JWT authentication with automatic token refresh
- User registration and email confirmation
- Password reset and change functionality
- Profile management with avatar upload
- Route protection and middleware
- Form validation with Zod
- Error handling and user feedback
- Type-safe API client with TypeScript
- Modern React patterns (hooks, context, Zustand)

Follow the structure and patterns outlined above to create a secure, maintainable, and user-friendly authentication system.
