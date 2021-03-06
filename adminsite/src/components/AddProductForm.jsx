import { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useHistory } from 'react-router-dom';
import { add_product, selectBrandList, get_Brand_List } from "../store/Product-slice";
import { get_Categories, selectCategories } from "../store/category-slice";

const AddProductForm = () => {
    const history = useHistory();

    const categories = useSelector(selectCategories);
    const brands = useSelector(selectBrandList);

    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(get_Categories());
        dispatch(get_Brand_List());
    }, [dispatch]);

    useEffect(() => {
        try {
            if (product.categoryId == 0 || product.brandId == 0) {
                product.brandId = brands[0].id;
                product.categoryId = categories[0].id
                setProduct({
                    ...product,
                    brandId: brands[0].id,
                    categoryId: categories[0].id
                });
            }
        } catch (error) {

        }
    });

    const [product, setProduct] = useState({
        productName: "",
        Price: 0,
        description: "",
        categoryId: 0,
        brandId: 0
    });

    const [formData, setFormData] = useState(new FormData());

    const [image, setImage] = useState({});

    const handleChangeFileImages = (e) => {
        const formData = new FormData();
        if (e.target.files) {
            let file = e.target.files[0];
            formData.append('ThumbnailImageUrl', file);
        }
        setFormData(formData)
    }

    function handleChange(evt) {
        const value = evt.target.value;
        setProduct({
            ...product,
            [evt.target.name]: value
        });
    }

    const handleSubmit = async (e) => {
        try {
            e.preventDefault();
            const formDataSubmit = formData;
            formDataSubmit.append('Name', product.productName);
            formDataSubmit.append('Description', product.description);
            formDataSubmit.append('Price', product.Price);
            formDataSubmit.append('CategoryId', product.categoryId);
            formDataSubmit.append('BrandId', product.brandId);
            dispatch(add_product(formDataSubmit))
            history.push("/")
        } catch (error) {
            console.log(error)
        }

    };

    return (
        <div>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <label>Product Name</label>
                    <input type="text" className="form-control" name="productName" value={product.productName} onChange={handleChange} required />
                </div>
                <div className="form-group">
                    <label>Price</label>
                    <input type="number" className="form-control" name="Price" value={product.Price} onChange={handleChange} />
                </div>
                <div className="form-group" >
                    <label>Description</label>
                    <textarea className="form-control" onChange={handleChange} value={product.description} name="description" rows="10" placeholder="Please type description of product"></textarea>
                </div>
                <div>
                    <label>Category </label>
                    <select className="form-control" name="categoryId" onChange={handleChange} required>
                        {
                            categories.map(item => {
                                return (
                                    <option value={item.id}>{item.name}</option>
                                )
                            })
                        }
                    </select>
                </div>
                <div>
                    <label>Brand</label>
                    <select className="form-control" name="brandId" onChange={handleChange} required>
                        {
                            brands.map(item => {
                                return (
                                    <option value={item.id}>{item.name}</option>
                                )
                            })
                        }
                    </select>
                </div>
                <div className="form-group">
                    <label>Image</label>
                    <input type='file' multiple className="form-control" onChange={handleChangeFileImages} required />
                </div>
                <button type="submit" className="btn btn-primary">Submit</button>
            </form>
        </div>
    );
};

export default AddProductForm;