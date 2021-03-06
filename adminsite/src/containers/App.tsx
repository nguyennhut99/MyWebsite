import { BrowserRouter, Switch, Route } from "react-router-dom";
import NavMenu from "./NavMenu";
import Auth from "./Auth";
import VerticalTabs from "../components/VerticalTabs"
import ProductTable from '../components/ProductTable'
import CategoryTable from '../components/CategoryTable'
import 'bootstrap/dist/css/bootstrap.min.css';
import AddProductForm from "../components/AddProductForm";
import UpdateProductForm from "../components/UpdateProductForm";
import UpdateCategory from "../components/UpdateCategory";
import UserManagement from "../components/UserManagement";
import UserDetail from "../components/UserOrder";
import OrderDetail from "../components/OrderDetail";
import { useEffect } from "react";

const App = () => {
  useEffect(() => {
    window.onload = () => {
      localStorage.setItem("__token", "");
    }
  });
  return (
    <BrowserRouter basename={"/"}>
      <NavMenu />
      <div className="row">
        <VerticalTabs />
        <div className="col-sm-10 col-md-10 col-lg-10 col-xl-10">
          <Switch>
            <Route path="/update-product" component={UpdateProductForm} />
            <Route path="/add-product" component={AddProductForm} />
            <Route path="/authentication" component={Auth} />
            <Route path="/category" component={CategoryTable} />
            <Route path="/update-category" component={UpdateCategory} />
            <Route path="/user-management" component={UserManagement} />
            <Route path="/user-detail" component={UserDetail} />
            <Route path="/order-detail" component={OrderDetail} />
            <Route path="/" component={ProductTable} />
          </Switch>
        </div>
      </div>
    </BrowserRouter >
  );
};

export default App;