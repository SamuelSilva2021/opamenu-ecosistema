export function OpaMenuLogo({ className = "", isDark = false, size = "default" }: { className?: string, isDark?: boolean, size?: "default" | "large" | "small" }) {
    const isLarge = size === "large";
    const isSmall = size === "small";

    const getIconSize = () => {
        if (isLarge) return "w-32 h-32";
        if (isSmall) return "w-9 h-9";
        return "w-12 h-12";
    };

    const getSvgSize = () => {
        if (isLarge) return "64";
        if (isSmall) return "18";
        return "24";
    };

    const getTextSize = () => {
        if (isLarge) return "text-6xl";
        if (isSmall) return "text-xl";
        return "text-2xl";
    };

    return (
        <div className={`flex items-center ${isLarge ? "gap-6" : isSmall ? "gap-2" : "gap-3"} ${className}`}>
            {/* Icon - Fork and Knife in a circular badge */}
            <div className={`relative flex items-center justify-center rounded-full bg-[#F37021] shadow-lg shrink-0 ${getIconSize()}`}>
                <svg
                    width={getSvgSize()}
                    height={getSvgSize()}
                    viewBox="0 0 24 24"
                    fill="none"
                    xmlns="http://www.w3.org/2000/svg"
                    className="text-white"
                >
                    {/* Fork */}
                    <path
                        d="M8 2V8C8 9.1 7.1 10 6 10C4.9 10 4 9.1 4 8V2"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                    />
                    <path
                        d="M6 10V22"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                    />
                    <path
                        d="M6 2V8"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                    />
                    {/* Knife */}
                    <path
                        d="M16 2L16 8C16 10 17 11 18 11C19 11 20 10 20 8V2"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                    />
                    <path
                        d="M18 11V22"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                    />
                </svg>
            </div>

            {/* Text Logo */}
            <div className="flex flex-col leading-none">
                <span className={`${getTextSize()} font-bold text-[#F37021]`}>
                    opa<span className={isDark ? "text-white" : "text-gray-800"}>menu</span>
                </span>
                {!isSmall && (
                    <span className={`${isLarge ? "text-xl mt-2" : "text-[10px] mt-1"} text-gray-500 font-medium tracking-wide`}>CARDÁPIO DIGITAL</span>
                )}
            </div>
        </div>
    );
}
