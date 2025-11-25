import Loading from "@/components/Loading";
import { useAuth } from "@/contexts/AuthContext";
import { useDynamicPartials, type ModuleLoader } from "@/hooks/use-partials";

const partialModules = import.meta.glob("./partials/*.tsx") as ModuleLoader;

const Profile = () => {
  const { user, isLoading, refetch } = useAuth();

  const partials = useDynamicPartials({
    partialModules,
    user,
    reverseOrder: false, 
  });

  if (isLoading || !user) {
    return <Loading />;
  }

  return (
    <div className="flex flex-col gap-10 max-w-3xl">
      {partials.map(({ key, Component }) => (
        <div key={key}>
          <Component user={user} refetchUser={refetch} />
        </div>
      ))}
    </div>
  );
};

export default Profile;
