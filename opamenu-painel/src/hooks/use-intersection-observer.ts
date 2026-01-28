import { useCallback, useRef, useState } from 'react';

export function useIntersectionObserver(options?: IntersectionObserverInit) {
  const [isIntersecting, setIsIntersecting] = useState(false);
  const observer = useRef<IntersectionObserver | null>(null);

  const ref = useCallback((node: Element | null) => {
    if (observer.current) {
      observer.current.disconnect();
      observer.current = null;
    }

    if (node) {
      observer.current = new IntersectionObserver(([entry]) => {
        setIsIntersecting(entry.isIntersecting);
      }, options);
      observer.current.observe(node);
    }
  }, [options?.root, options?.rootMargin, options?.threshold]);

  return { ref, isIntersecting };
}
