import { useDispatch, useSelector } from "react-redux";
import { useHistory } from 'react-router-dom';
import { useEffect } from "react";
import { get_product_list, selectProductList, delete_product } from "../store/Product-slice";

const ProductTable = () => {

  const history = useHistory();

  const ProductList = useSelector(selectProductList);

  const dispatch = useDispatch();
  useEffect(() => {
    dispatch(get_product_list());
  }, [dispatch]);

  return (
    <div>
      <button className='btn btn-primary' onClick={() => history.push("/add-product")}>ADD product</button>
      <div className="table-item">
        <table className="table mt-4 ">
          <tr>
            <th>Id</th>
            <th>Thumbnail Image</th>
            <th>Name</th>
            <th>Price</th>
            <th>Description</th>
            <th>Create Date</th>
            <th>Modify Date</th>
            <th></th>
          </tr>
          {ProductList.map((product) => (
            <tr>
              <td>{product.id}</td>
              <td><img src={product.thumbnailImageUrl} alt="..." className="img-thumbnail" /></td>
              <td>{product.name}</td>
              <td>{product.price}</td>
              <td>{product.description}</td>
              <td>{product.createDate}</td>
              <td>{product.modifyDate}</td>
              <td>
                <button className="btn btn-success" onClick={() => history.push("/update-product?id=" + product.id)}>Cập nhật</button>

                <button className="btn btn-danger" onClick={() => dispatch(delete_product(product.id))} > Xóa</button>
              </td>
            </tr>
          ))}

        </table>
      </div>
    </div>
  );
};

export default ProductTable;