import { ThemeProvider, QueryProvider, RouterProvider } from './app/providers';
import { AppInitializationProvider } from './app/providers/AppInitializationProvider';
import { AppRoutes } from './app/routes';

function App() {
  return (
    <QueryProvider>
      <ThemeProvider>
        <RouterProvider>
          <AppInitializationProvider>
            <AppRoutes />
          </AppInitializationProvider>
        </RouterProvider>
      </ThemeProvider>
    </QueryProvider>
  );
}

export default App;
