import { useLocation } from "react-router-dom";
import { useEffect } from "react";

interface NotFoundProps {
  title?: string;
  message?: string;
  showHomeLink?: boolean;
}

const NotFound = ({ 
  title = "404", 
  message = "Oops! Page not found", 
  showHomeLink = true 
}: NotFoundProps) => {
  const location = useLocation();

  useEffect(() => {
    console.error(
      "404 Error: User attempted to access non-existent route:",
      location.pathname
    );
  }, [location.pathname]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="text-center">
        <h1 className="text-4xl font-bold mb-4">{title}</h1>
        <p className="text-xl text-gray-600 mb-4">{message}</p>
        {showHomeLink && (
          <a href="/" className="text-blue-500 hover:text-blue-700 underline">
            Return to Home
          </a>
        )}
      </div>
    </div>
  );
};

export default NotFound;
