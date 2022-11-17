import { useAuth } from "react-oidc-context";
import { UserManager, User } from "oidc-client-ts";
import logo from './logo.svg';
import './App.css';
import Notes from "./Notes";

function getUser() {
  const oidcStorage = sessionStorage.getItem('oidc.user:https://localhost:5001:spa-user-ui')
  if (!oidcStorage) {
    return null;
  }

  return User.fromStorageString(oidcStorage);
}

function App() {
  const auth = useAuth();
  
  // const oidcConfig = {
  //   authority: "https://localhost:5001",
  //   client_id: "spa-user-ui",
  //   redirect_uri: "https://localhost:3000/callback.html",
  // };
  // const userManager = new UserManager(oidcConfig);
  // userManager.getUser().then(u => {
  //   alert(JSON.stringify(u?.profile));
  // });

  switch (auth.activeNavigator) {
    case "signinSilent":
      return <div>Signing you in...</div>;
    case "signoutRedirect":
      return <div>Signing you out...</div>;
  }

  if (auth.isLoading) {
    return <div>Loading...</div>;
  }

  if (auth.error) {
    return <div>Oops... {auth.error.message}</div>;
  }

  if (auth.isAuthenticated) {
    const user = getUser();
    const token = user?.access_token;

    return (
      <div className="App">
        Hello {auth.user?.profile.sub}{" "}
        <button onClick={() => void auth.removeUser()}>Log out</button>
        <div>
          Your access token is:
          <div>
            <textarea className="token-display" defaultValue={token}/>
          </div>
        </div>

        <Notes/>
      </div>
    );
  }

  return <button onClick={() => void auth.signinRedirect()}>Log in</button>;
}

export default App;
