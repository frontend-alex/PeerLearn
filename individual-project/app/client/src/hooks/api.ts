import { API } from "@/lib/config";
import axios, { type AxiosInstance } from "axios";
import { redirect } from "react-router-dom";


const api: AxiosInstance = axios.create({
  baseURL: API.BASE_URL,
  withCredentials: true,
});


api.interceptors.response.use(
  (response) => {
    if (!response.data.success) {
      return Promise.reject(response.data.message);
    }
    return response;
  },
  (error) => {
    if (error.response?.status === 401) {
      redirect("/login"); 
    }
    return Promise.reject(error);
  }
);

export default api;
